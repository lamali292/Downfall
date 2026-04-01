using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class MassRepurpose : SlimeBossCardModel
{
    public MassRepurpose() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        
    }
    // TODO: Implement
}