using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Downfall.DownfallCode.CustomEnums;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Rewards;
using MegaCrit.Sts2.Core.GameActions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Guardian.GuardianCode.Rewards;

public class GemFinderReward(int choosable, int choices, Player player) : CustomReward(player)
{
    [CustomEnum] public static RewardType GemFinderRewardType;

    // Same source CardReward falls back to; both sides of the sync use it.
    private readonly PlayerChoiceSynchronizer _synchronizer =
        RunManager.Instance.PlayerChoiceSynchronizer;

    private NSimpleCardSelectScreen? _currentlyShownScreen;

    private List<GemModel> Gems { get; } = [];

    // Deterministic placeholder before population — no Random.Shared in getters.
    protected override string IconPath => Gems.Count > 0
        ? Gems[0].IconPath
        : GuardianModelDb.AllGems.First().IconPath;

    protected override RewardType RewardType => GemFinderRewardType;

    public override LocString Description
    {
        get
        {
            var desc = new LocString("gameplay_ui", "COMBAT_REWARD_ADD_GEMS");
            desc.Add("Amount", choosable);
            return desc;
        }
    }

    public override bool IsPopulated => Gems.Count > 0;
    public override CreateRewardFromSave<CustomReward> DeserializeMethod => Deserialize;

    public override void Populate()
    {
        // Match CardReward: never re-roll an already-populated reward.
        if (Gems.Count > 0)
            return;

        var gemsByRarity = GuardianModelDb.AllGems
            .GroupBy(g => g.Rarity)
            .ToDictionary(g => g.Key, g => g.ToList());

        // Never ask for more unique gems than exist — this was an infinite loop.
        var target = Math.Min(choices, GuardianModelDb.AllGems
            .DistinctBy(g => g.Id.Entry).Count());

        var rng = Player.PlayerRng.Rewards;
        var safety = 0;
        while (Gems.Count < target && safety++ < 10_000)
        {
            var roll = rng.NextInt(100);
            var rarity = roll < 55 ? CardRarity.Common
                : roll < 85 ? CardRarity.Uncommon
                : CardRarity.Rare;

            if (!gemsByRarity.TryGetValue(rarity, out var pool) || pool.Count == 0)
                continue;

            var candidate = rng.NextItem(pool);
            if (candidate == null)
                continue;

            // Remove from the pool instead of a 'seen' set, so exhausted
            // rarities stop being re-rollable dead ends.
            pool.Remove(candidate);
            Gems.Add(candidate);
        }

        if (Gems.Count < target)
            Log.Error($"GemFinderReward only populated {Gems.Count}/{target} gems!");
    }

    protected override async Task<bool> OnSelect()
    {
        // Both clients build this from Gems — the lists MUST be identical,
        // because remotes dereference synced indices into their own copy.
        var cards = Gems.Select(e => e.ToCard).ToList();
        var chosenIndices = new List<int>();

        if (LocalContext.IsMe(Player))
        {
            // Screen exists only on the owning client, like CardReward.
            var prefs = new CardSelectorPrefs(
                DownfallCardSelectorPrefs.ToDeckSelectionPrompt, 0, choosable);
            _currentlyShownScreen = NSimpleCardSelectScreen.Create(cards, prefs);
            NOverlayStack.Instance?.Push(_currentlyShownScreen);

            var selectedCards = (await _currentlyShownScreen.CardsSelected()).ToList();
            CleanupScreen();

            foreach (var idx in selectedCards.Select(card => cards.IndexOf(card)))
            {
                if (idx >= 0)
                    chosenIndices.Add(idx);
                else
                    Log.Error("GemFinderReward: selected card not found in offered list!");
            }

            // Protocol: one synced index per pick, then a null terminator.
            foreach (var idx in chosenIndices)
            {
                var choiceId = _synchronizer.ReserveChoiceId(Player);
                _synchronizer.SyncLocalChoice(Player, choiceId,
                    PlayerChoiceResult.FromIndex(idx));
            }
            var endId = _synchronizer.ReserveChoiceId(Player);
            _synchronizer.SyncLocalChoice(Player, endId,
                PlayerChoiceResult.FromIndex(null));
        }
        else
        {
            // Remote clients: no screen, just consume choices until terminator.
            // ReserveChoiceId is called in the same order on both sides, so IDs align.
            while (true)
            {
                var choiceId = _synchronizer.ReserveChoiceId(Player);
                var idx = (await _synchronizer.WaitForRemoteChoice(Player, choiceId))
                    .AsIndexOrNull();
                if (!idx.HasValue)
                    break;
                if (idx.Value < 0 || idx.Value >= cards.Count)
                {
                    Log.Error($"GemFinderReward: bad remote index {idx.Value} " +
                              $"for {cards.Count} gems!");
                    continue;
                }
                chosenIndices.Add(idx.Value);
            }
        }
        
        if (chosenIndices.Count <= 0) return true;
        var mutable = chosenIndices
            .Select(i => Player.RunState.CreateCard(cards[i], Player))
            .ToList();
        var result = await CardPileCmd.Add(mutable, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result);

        return true;
    }

    private void CleanupScreen()
    {
        if (_currentlyShownScreen == null)
            return;
        NOverlayStack.Instance?.Remove(_currentlyShownScreen);
        _currentlyShownScreen = null;
    }

    public override void OnSkipped() => CleanupScreen();

    public override void MarkContentAsSeen()
    {
    }

    private static CustomReward Deserialize(SerializableReward save, Player player)
        => new GemFinderReward(save.GoldAmount, save.OptionCount, player);

    public override SerializableReward ToSerializable() => new()
    {
        RewardType = GemFinderRewardType,
        GoldAmount = choosable,
        OptionCount = choices
    };
}