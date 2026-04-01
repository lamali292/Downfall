using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Gremlins.Common;

[Pool(typeof(GremlinsCardPool))]
public class Pinprick : GremlinsCardModel
{
    public Pinprick() : base(0, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}