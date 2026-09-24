using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.CardRewardAlternatives;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves.Runs;
using MegaCrit.Sts2.Core.TestSupport;

namespace Collector.CollectorCode.Rewards;

public class CollectibleChoiceReward(int cardCount, bool shouldUpgrade, Player player) : CustomReward(player)
{
    [CustomEnum]
    public static RewardType CollectibleChoiceRewardType;

    // Same source CardReward falls back to; both sides of the sync use it.
    private readonly PlayerChoiceSynchronizer _synchronizer =
        RunManager.Instance.PlayerChoiceSynchronizer;

    // No alternatives (e.g. skip/purge) — just the fanned card options.
    private static readonly List<CardRewardAlternative> _noAlternatives = [];

    private NCardRewardSelectionScreen? _currentlyShownScreen;

    private List<CardCreationResult> Options { get; } = [];

    public override void Populate()
    {
        // Match CardReward: never re-roll an already-populated reward.
        if (Options.Count > 0)
            return;

        var options = new CardCreationOptions([ModelDb.CardPool<CollectibleCardPool>()], CardCreationSource.Other, CardRarityOddsType.Uniform);
        var results = CardFactory.CreateForReward(Player, cardCount, options).ToList();
        if (shouldUpgrade)
        {
            foreach (var result in results)
            {
                CardCmd.Upgrade(result.Card);
            }
        }

        Options.AddRange(results);
    }

    protected override async Task<bool> OnSelect()
    {
        // Both clients build this from Options — the lists MUST be identical,
        // because remotes dereference the synced index into their own copy.
        var options = Options;
        int? chosenIndex;

        var choiceId = _synchronizer.ReserveChoiceId(Player);
        if (LocalContext.IsMe(Player))
        {
            // Screen exists only on the owning client, like CardReward.
            if (TestMode.IsOn)
            {
                var selection = CardSelectCmd.Selector?.GetSelectedCardReward(options, _noAlternatives);
                chosenIndex = selection?.card == null
                    ? null
                    : options.FindIndex(o => o.Card == selection.Value.card);
            }
            else
            {
                _currentlyShownScreen = NCardRewardSelectionScreen.ShowScreen(options, _noAlternatives);
                chosenIndex = _currentlyShownScreen != null
                    ? await _currentlyShownScreen.OptionSelected()
                    : null;
                CleanupScreen();
            }

            _synchronizer.SyncLocalChoice(Player, choiceId, PlayerChoiceResult.FromIndex(chosenIndex));
        }
        else
        {
            chosenIndex = (await _synchronizer.WaitForRemoteChoice(Player, choiceId)).AsIndexOrNull();
        }

        if (!chosenIndex.HasValue)
            return false;

        if (chosenIndex.Value < 0 || chosenIndex.Value >= options.Count)
        {
            Log.Error($"CollectibleChoiceReward: bad choice index {chosenIndex.Value} " +
                      $"for {options.Count} cards!");
            return false;
        }

        // CardFactory.CreateForReward already returns player-owned mutable instances
        // (not canonical templates) — CreateCard() on them would fail AssertCanonical.
        var chosenCard = options[chosenIndex.Value].Card;
        var result = await CardPileCmd.Add(chosenCard, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result);

        RecordChoice(chosenCard, wasPicked: true);
        for (var i = 0; i < options.Count; i++)
            if (i != chosenIndex.Value)
                RecordChoice(options[i].Card, wasPicked: false);

        return true;
    }

    private void RecordChoice(CardModel card, bool wasPicked)
    {
        Player.RunState.CurrentMapPointHistoryEntry?
            .GetEntry(Player.NetId)
            .CardChoices.Add(new CardChoiceHistoryEntry(card, wasPicked));
    }

    private void CleanupScreen()
    {
        if (_currentlyShownScreen == null)
            return;
        NOverlayStack.Instance?.Remove(_currentlyShownScreen);
        _currentlyShownScreen = null;
    }

    public override void OnSkipped()
    {
        foreach (var option in Options)
            RecordChoice(option.Card, wasPicked: false);
        CleanupScreen();
    }

    public override void MarkContentAsSeen()
    {
    }

    private static string RewardIcon => ImageHelper.GetImagePath("ui/reward_screen/reward_icon_special_card.png");
    protected override string IconPath => RewardIcon;

    protected override RewardType RewardType => CollectibleChoiceRewardType;
    public override LocString Description
    {
        get
        {
            var desc = new LocString("gameplay_ui", "COLLECTIBLE_CHOICE_REWARD");
            desc.Add("IsUpgraded", shouldUpgrade);
            return desc;
        }
    }

    public override bool IsPopulated => Options.Count > 0;
    public override CreateRewardFromSave<CustomReward> DeserializeMethod => Deserialize;

    private static CustomReward Deserialize(SerializableReward save, Player player)
    {
        return new CollectibleChoiceReward(save.OptionCount, save.WasGoldStolenBack, player);
    }

    public override SerializableReward ToSerializable()
    {
        return new SerializableReward
        {
            RewardType = CollectibleChoiceRewardType,
            OptionCount = cardCount,
            WasGoldStolenBack = shouldUpgrade
        };
    }
}
