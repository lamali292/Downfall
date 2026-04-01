using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Basic;

[Pool(typeof(SlimeBossCardPool))]
public class Split : SlimeBossCardModel
{
    public Split() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        
    }
    // TODO: Implement
}