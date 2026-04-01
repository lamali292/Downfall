using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Basic;

[Pool(typeof(CollectorCardPool))]
public class FuelTheFire : CollectorCardModel
{
    public FuelTheFire() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        
    }
    // TODO: Implement
}