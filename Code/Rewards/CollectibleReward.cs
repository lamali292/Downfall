using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Downfall.Code.Core.Collector;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Downfall.Code.Rewards;

public class CollectibleReward(CardModel card, Player player) : CustomReward(player)
{
    [CustomEnum] public static RewardType CustomCardRewardType;

    private bool _wasTaken;
    protected override RewardType RewardType => CustomCardRewardType;

    private static string RewardIcon => ImageHelper.GetImagePath("ui/reward_screen/reward_icon_special_card.png");
    protected override string IconPath => RewardIcon;

    public override int RewardsSetIndex => 9;

    public override LocString Description
    {
        get
        {
            var desc = Player.CanAffordEssence(3)
                ? new LocString("gameplay_ui", "COLLECTIBLE_REWARD")
                : new LocString("gameplay_ui", "COLLECTIBLE_REWARD_CANT_AFFORD");
            desc.Add("Card", card.Title);
            desc.Add("essence", 3);
            desc.Add("current", Player.GetEssence());
            return desc;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard(card)];

    public override bool IsPopulated => card != null;

    public override SerializableCustomReward<CustomReward> SerializeMethod => Deserialize;

    public override Task Populate()
    {
        return Task.CompletedTask;
    }

    protected override async Task<bool> OnSelect()
    {
        if (!Player.CanAffordEssence(3)) return false;
        CollectiblesModel.SyncAdd(Player, card, 3);
        _wasTaken = true;
        return true;
    }

    public override void OnSkipped()
    {
        if (_wasTaken || LocalContext.NetId == null) return;
        Player.RunState.CurrentMapPointHistoryEntry?
            .GetEntry(LocalContext.NetId.Value)
            .CardChoices.Add(new CardChoiceHistoryEntry(card, false));
        RunManager.Instance.RewardSynchronizer.GameService()?.SendMessage(new CollectibleRewardMessage
        {
            wasSkipped = true,
            Location = RunManager.Instance.RewardSynchronizer.MessageBuffer()!.CurrentLocation,
            Card = card.ToSerializable(),
            EssenceCost = 0
        });
    }

    public override SerializableReward ToSerializable()
    {
        return new SerializableReward
        {
            RewardType = CustomCardRewardType,
            SpecialCard = card.ToSerializable()
        };
    }

    public static CustomReward Deserialize(SerializableReward save, Player player)
    {
        var cardModel = CardModel.FromSerializable(save.SpecialCard!);
        CollectiblesModel.AddCollectible(player, cardModel);
        return new CollectibleReward(cardModel, player);
    }

    public override void MarkContentAsSeen()
    {
    }
}