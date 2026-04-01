using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Basic;

[Pool(typeof(GremlinsCardPool))]
public class StrikeGremlins : GremlinsCardModel
{
    public StrikeGremlins() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}