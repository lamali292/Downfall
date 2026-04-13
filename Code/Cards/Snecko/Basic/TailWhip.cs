using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Snecko.Basic;

[Pool(typeof(SneckoCardPool))]
public class TailWhip : SneckoCardModel
{
    public TailWhip() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}