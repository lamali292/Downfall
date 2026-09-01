using BaseLib.Abstracts;
using BaseLib.Patches.Content;
using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs.History;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Collector.CollectorCode.Rewards;

public class CollectibleReward(ModelId monsterModel, Player player) : CustomReward(player)
{
    [CustomEnum] public static RewardType CustomCardRewardType;
    
    protected override RewardType RewardType => CustomCardRewardType;

    private static string RewardIcon => ImageHelper.GetImagePath("ui/reward_screen/reward_icon_special_card.png");
    protected override string IconPath => RewardIcon;

    public override int RewardsSetIndex => 9;


    public override LocString Description
    {
        get
        {
            var desc = new LocString("gameplay_ui", "COLLECTIBLE_REWARD");
            desc.Add("Card", Card.Title);
            return desc;
        }
    }

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        [HoverTipFactory.FromCard(Card)];

    public override bool IsPopulated => _card != null;

    public override CreateRewardFromSave<CustomReward> DeserializeMethod => Deserialize;

    public override void Populate()
    {
        _card = ModelDb.CardPool<CollectibleCardPool>().AllCards.FirstOrDefault(c => c is ICollectible g && g.GetMonsterModel().Id == monsterModel);
    }

    private CardModel? _card;
    private CardModel Card => _card!;

    protected override async Task<bool> OnSelect()
    {  
        if (LocalContext.NetId == null) return false;
        Card.AssertCanonical();
        var runCard = Player.RunState.CreateCard(Card, Player);
        var result = await CardPileCmd.Add(runCard, PileType.Deck);
        CardCmd.PreviewCardPileAdd(result, 0.4f);
        runCard.AssertMutable();
        
        Player.RunState.CurrentMapPointHistoryEntry?
            .GetEntry(LocalContext.NetId.Value)
            .CardChoices.Add(new CardChoiceHistoryEntry(runCard, true));
        return true;
    }
    

    public override void OnSkipped()
    {
        if (LocalContext.NetId == null) return;
        var runCard = Player.RunState.CreateCard(Card, Player);
        Player.RunState.CurrentMapPointHistoryEntry?
            .GetEntry(LocalContext.NetId.Value)
            .CardChoices.Add(new CardChoiceHistoryEntry(runCard, false));
    }

    public override SerializableReward ToSerializable()
    {
        return new SerializableReward
        {
            RewardType = CustomCardRewardType,
            PredeterminedModelId = monsterModel
        };
    }

    private static CustomReward Deserialize(SerializableReward save, Player player)
    {
        return new CollectibleReward(save.PredeterminedModelId, player);
    }

    public override void MarkContentAsSeen()
    {
    }
}