using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Basic;

[Pool(typeof(CollectorCardPool))]
public class StrikeCollector : CollectorCardModel
{
    public StrikeCollector() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}