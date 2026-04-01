using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Common;

[Pool(typeof(SlimeBossCardPool))]
public class LeechEnergy : SlimeBossCardModel
{
    public LeechEnergy() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}