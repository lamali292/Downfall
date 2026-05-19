using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Rare;

/// <summary>
///     Deal 15 damage to ALL enemies. Stun any that don't intend to attack. Exhaust.
///     Upgrade: 20 damage.
/// </summary>
public sealed class RoundhouseKick : HermitCardModel
{
    public RoundhouseKick() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(13, 5);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        await CommonActions.CardAttack(this, play)
            .WithHermitBluntHeavyHitFx()
            .Execute(ctx);

        // Stun enemies that don't intend to attack
        foreach (var enemy in CombatState!.HittableEnemies)
        {
            if (enemy.IsDead) continue;
            var monster = enemy.Monster;
            if (monster == null) continue;

            // Check if the monster does NOT intend to attack
            if (!monster.IntendsToAttack) await CreatureCmd.Stun(enemy);
        }
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Rare
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   CanonicalKeywords removed → WithKeyword(...) in constructor
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(13, 5), WithKeyword(CardKeyword.Exhaust)
 */