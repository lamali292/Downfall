using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Guardian.Basic;

[Pool(typeof(GuardianCardPool))]
public class DefendGuardian : GuardianCardModel
{
    public DefendGuardian() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        
    }
    // TODO: Implement
}