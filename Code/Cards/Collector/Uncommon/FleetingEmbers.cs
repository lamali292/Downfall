using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class FleetingEmbers : CollectorCardModel
{
    public FleetingEmbers() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        
    }
    // TODO: Implement
}