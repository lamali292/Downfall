using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Cards.SlimeBoss.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class RallyTheTroops : SlimeBossCardModel
{
    public RallyTheTroops() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        
    }
    // TODO: Implement
}