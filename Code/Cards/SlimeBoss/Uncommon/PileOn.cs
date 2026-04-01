using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class PileOn : SlimeBossCardModel
{
    public PileOn() : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}