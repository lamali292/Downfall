using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Common;

[Pool(typeof(GremlinsCardPool))]
public class Presto : GremlinsCardModel
{
    public Presto() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}