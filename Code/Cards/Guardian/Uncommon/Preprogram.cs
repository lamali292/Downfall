using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Preprogram : GuardianCardModel
{
    public Preprogram() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        
    }
    // TODO: Implement
}