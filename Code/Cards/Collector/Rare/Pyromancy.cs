using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class Pyromancy : CollectorCardModel
{
    public Pyromancy() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        
    }
    // TODO: Implement
}