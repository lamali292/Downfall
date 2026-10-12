using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Guardian.GuardianCode.Rewards;

/// <summary>
/// A single, predetermined Gem. Unlike <see cref="GemFinderReward"/> there is no selection screen:
/// the gem is named in the reward text, hovering shows its card, and leaving it unclaimed skips it.
/// </summary>
public class GemReward(Player player, GemModel? gem = null) : CustomReward(player)
{
    [CustomEnum] public static RewardType GemRewardType;

    private GemModel? _gem = gem;

    protected override RewardType RewardType => GemRewardType;

    // Deterministic placeholder before population — no Random.Shared in getters.
    protected override string IconPath => (_gem ?? GuardianModelDb.AllGems.First()).IconPath;

    public override bool IsPopulated => _gem != null;
    public override CreateRewardFromSave<CustomReward> DeserializeMethod => Deserialize;

    public override LocString Description
    {
        get
        {
            var desc = new LocString("gameplay_ui", "COMBAT_REWARD_ADD_GEM");
            desc.Add("Gem", _gem?.Title.GetFormattedText() ?? "");
            return desc;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        _gem == null ? [] : [HoverTipFactory.FromCard(_gem.ToCard)];

    public override void Populate()
    {
        if (_gem != null)
            return;

        var gemsByRarity = GuardianModelDb.AllGems
            .GroupBy(g => g.Rarity)
            .ToDictionary(g => g.Key, g => g.ToList());
        var rng = Player.PlayerRng.Rewards;
        // Bounded: a missing rarity bucket just re-rolls.
        for (var attempt = 0; attempt < 1_000 && _gem == null; attempt++)
        {
            if (gemsByRarity.TryGetValue(GemFinderReward.RollRarity(rng), out var pool) && pool.Count > 0)
                _gem = rng.NextItem(pool);
        }

        _gem ??= GuardianModelDb.AllGems.First();
    }

    protected override async Task<bool> OnSelect()
    {
        var card = Player.RunState.CreateCard(_gem!.ToCard, Player);
        var result = await CardPileCmd.Add(card, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result);
        return true;
    }

    public override void OnSkipped()
    {
    }

    public override void MarkContentAsSeen()
    {
    }

    private static CustomReward Deserialize(SerializableReward save, Player player)
    {
        var card = ModelDb.GetByIdOrNull<CardModel>(save.PredeterminedModelId);
        var gem = (card as IGemCard)?.CanonicalGemModel;
        return new GemReward(player, gem);
    }

    public override SerializableReward ToSerializable()
    {
        return new SerializableReward
        {
            RewardType = GemRewardType,
            PredeterminedModelId = _gem?.ToCard.Id ?? ModelId.none
        };
    }
}
