using BaseLib.Utils;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class TheContract : CollectorRelicModel
{
    public TheContract() : base(RelicRarity.Uncommon)
    {
        WithCards(5);
    }

    /*
    private bool ActivatedThisCombat
    {
        get;
        set
        {
            AssertMutable();
            field = value;
        }
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom) return Task.CompletedTask;
        ActivatedThisCombat = false;
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (ActivatedThisCombat || cardPlay.Card.Owner != Owner || !cardPlay.Card.VisualCardPool.IsColorless)   return;
        await PlayerCmd.GainEnergy(cardPlay.Resources.EnergySpent, Owner);
        Flash();
        ActivatedThisCombat = true;
    }
    */
    
    public override async Task AfterObtained()
    {
        CardCreationOptions options = new CardCreationOptions([ModelDb.CardPool<CollectibleCardPool>()], CardCreationSource.Other, CardRarityOddsType.RegularEncounter);
        var reward = new CardReward(options, 5, Owner);
        foreach (var cardCreationResult in reward.Cards)
        {
            Upgrade(cardCreationResult);
        }
        await RewardsCmd.OfferCustom(Owner, [reward]);
    }
    
    private CardModel Upgrade(CardModel cardModel)
    {
        CardCmd.Upgrade(cardModel);
        return cardModel;
    }
}