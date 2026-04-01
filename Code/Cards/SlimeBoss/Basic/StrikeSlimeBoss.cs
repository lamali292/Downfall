using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Basic;

[Pool(typeof(SlimeBossCardPool))]
public class StrikeSlimeBoss : SlimeBossCardModel
{
    public StrikeSlimeBoss() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}