using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Rewards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Collector.CollectorCode.Relics;

[Pool(typeof(CollectorRelicPool))]
public class TheContract : CollectorRelicModel
{
    public TheContract() : base(RelicRarity.Uncommon)
    {
        WithCards(5);
    }
    
    public override async Task AfterObtained()
    {
        await RewardsCmd.OfferCustom(Owner, [new CollectibleChoiceReward(DynamicVars.Cards.IntValue, true, Owner)]);
    }
    
}