using BaseLib.Patches.Content;
using Downfall.DownfallCode.Utils.CustomReward;
using Guardian.GuardianCode.Cards.Rare;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Guardian.GuardianCode.Rewards;

public class GemFinderReward(int count, Player player) : CustomReward(player)
{
    [CustomEnum] public static RewardType GemFinderRewardType;


    private NSimpleCardSelectScreen? _currentlyShownScreen;

    private List<GemModel> Gems { get; } = [];

    protected override string IconPath => Gems.Count > 0
        ? Gems.First().IconPath
        : GuardianModelDb.AllGems.ElementAt(Random.Shared.Next(GuardianModelDb.AllGems.Count())).IconPath;

    protected override RewardType RewardType => GemFinderRewardType;

    public override LocString Description
    {
        get
        {
            var desc = new LocString("gameplay_ui", "COMBAT_REWARD_ADD_GEMS");
            desc.Add("Amount", count);
            return desc;
        }
    }

    public override bool IsPopulated => Gems.Count > 0;
    public override SerializableCustomReward<CustomReward> SerializeMethod => Deserialize;

    public override Task Populate()
    {
        var gemsByRarity = GuardianModelDb.AllGems
            .GroupBy(g => g.Rarity)
            .ToDictionary(g => g.Key, g => g.ToList());

        var seen = new HashSet<string>();
        var rng = Player.PlayerRng.Rewards;
        while (Gems.Count < count)
        {
            var roll = rng.NextInt(100);
            var rarity = roll < 55 ? CardRarity.Common
                : roll < 85 ? CardRarity.Uncommon
                : CardRarity.Rare;

            if (!gemsByRarity.TryGetValue(rarity, out var pool)) continue;

            var candidate = rng.NextItem(pool);
            if (candidate == null || !seen.Add(candidate.Id.Entry)) continue;

            Gems.Add(candidate);
        }

        return Task.CompletedTask;
    }

    protected override async Task<bool> OnSelect()
    {
        var prefs = new CardSelectorPrefs(ModelDb.Card<GemFinder>().SelectionScreenPrompt, 0, Gems.Count);
        var cards = Gems.Select(e => e.ToCard).ToList();
        _currentlyShownScreen = NSimpleCardSelectScreen.Create(cards, prefs);
        NOverlayStack.Instance?.Push(_currentlyShownScreen);

        var selectedCards = (await _currentlyShownScreen.CardsSelected()).ToList();
        if (selectedCards.Count == 0)
        {
            RunManager.Instance.RewardSynchronizer.GameService()?.SendMessage(new CardsAddedMessage
            {
                Cards = [],
                Location = RunManager.Instance.RewardSynchronizer.MessageBuffer()!.CurrentLocation,
                wasSkipped = true
            });
            return true;
        }

        var mutable = selectedCards.Select(e => e.ToMutable()).ToList();
        foreach (var cardModel in mutable) Player.RunState.AddCard(cardModel, Player);
        var result = await CardPileCmd.Add(mutable, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result);
        var cardsAdded = result.Select(e => e.cardAdded.ToSerializable()).ToList();
        RunManager.Instance.RewardSynchronizer.GameService()?.SendMessage(new CardsAddedMessage
        {
            Cards = cardsAdded,
            Location = RunManager.Instance.RewardSynchronizer.MessageBuffer()!.CurrentLocation,
            wasSkipped = false
        });
        if (_currentlyShownScreen == null) return true;
        NOverlayStack.Instance?.Remove(_currentlyShownScreen);
        _currentlyShownScreen = null;
        return true;
    }

    public override void MarkContentAsSeen()
    {
    }

    private static CustomReward Deserialize(SerializableReward save, Player player1)
    {
        return new GemFinderReward(save.GoldAmount, player1);
    }

    public override SerializableReward ToSerializable()
    {
        return new SerializableReward
        {
            RewardType = GemFinderRewardType,
            GoldAmount = count
        };
    }
}