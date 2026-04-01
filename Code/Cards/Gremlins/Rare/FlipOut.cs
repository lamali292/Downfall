using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Rare;

[Pool(typeof(GremlinsCardPool))]
public class FlipOut : GremlinsCardModel
{
    public FlipOut() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}