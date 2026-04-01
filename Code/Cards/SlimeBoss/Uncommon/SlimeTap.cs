using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Uncommon;

[Pool(typeof(SlimeBossCardPool))]
public class SlimeTap : SlimeBossCardModel
{
    public SlimeTap() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        
    }
    // TODO: Implement
}