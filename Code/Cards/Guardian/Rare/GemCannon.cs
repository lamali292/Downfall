using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class GemCannon : GuardianCardModel
{
    public GemCannon() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}