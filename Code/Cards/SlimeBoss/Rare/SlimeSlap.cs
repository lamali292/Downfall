using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class SlimeSlap : SlimeBossCardModel
{
    public SlimeSlap() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}