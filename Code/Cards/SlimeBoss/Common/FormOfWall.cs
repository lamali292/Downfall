using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Common;

[Pool(typeof(SlimeBossCardPool))]
public class FormOfWall : SlimeBossCardModel
{
    public FormOfWall() : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        
    }
    // TODO: Implement
}