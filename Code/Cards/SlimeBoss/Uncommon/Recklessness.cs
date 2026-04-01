using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class Recklessness : SlimeBossCardModel
{
    public Recklessness() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        
    }
    // TODO: Implement
}