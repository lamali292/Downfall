using BaseLib.Utils;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Deal 6 damage to ALL enemies. Deals 2 more for each Curse card in all piles.
///     Upgrade: 9 damage.
/// </summary>
public sealed class Grudge : HermitCardModel
{
    public Grudge() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithCalculatedDamage(9, 2, CountCurses, ValueProp.Move, 0, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play)
            .WithHermitFireHitFx()
            .Execute(ctx);
    }

    private static decimal CountCurses(CardModel card, Creature? _)
    {
        return card.Owner.PlayerCombatState?.AllCards.Count(e => e.Type == CardType.Curse) ?? 0;
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   constructor: WithDamage(9, 0)
 */