using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace SlimeBoss.SlimeBossCode.Cards.Rare;

[Pool(typeof(SlimeBossCardPool))]
public class PerfectedTackle : SlimeBossCardModel
{
    public PerfectedTackle() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithTags(SlimeBossTag.Tackle);
        WithCalculatedDamage(30, 5, CountTwoEnergyAttacks, bonusUpgrade: 5);
    }

    private static decimal CountTwoEnergyAttacks(CardModel card, Creature? _)
    {
        return card.Owner.GetAllCombatCards.Count(SlimeBossCmd.IsTwoEnergyAttack);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
