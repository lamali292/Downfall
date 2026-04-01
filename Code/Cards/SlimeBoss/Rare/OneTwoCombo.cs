using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class OneTwoCombo : SlimeBossCardModel
{
    public OneTwoCombo() : base(0, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}