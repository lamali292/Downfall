using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Empower : CollectorCardModel
{
    public Empower() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        
    }
    // TODO: Implement
}