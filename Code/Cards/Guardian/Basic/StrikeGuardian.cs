using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Guardian.Basic;

[Pool(typeof(GuardianCardPool))]
public class StrikeGuardian : GuardianCardModel
{
    public StrikeGuardian() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}