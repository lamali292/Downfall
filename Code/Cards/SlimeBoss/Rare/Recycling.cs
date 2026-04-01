using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class Recycling : SlimeBossCardModel
{
    public Recycling() : base(1, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        
    }
    // TODO: Implement
}