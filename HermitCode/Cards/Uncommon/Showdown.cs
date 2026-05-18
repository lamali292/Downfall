using BaseLib.Utils;
using HermitMod.Utility;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Cards.Uncommon;

/// <summary>
///     Deal 12 damage. Play all Strikes in your hand.
///     Upgrade: 16 damage.
/// </summary>
public sealed class Showdown : HermitCardModel
{
    public Showdown() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithDamage(9, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun2();
        await CommonActions.CardAttack(this, play).WithHermitGunHitFx()
            .Execute(ctx);

        // Auto-play all Strikes in hand (match by type, not rarity — covers Hermit and base game Strikes)
        var strikes = PileType.Hand.GetPile(Owner).Cards
            .Where(c => c.Tags.Contains(CardTag.Strike))
            .ToList();

        foreach (var strike in strikes)
        {
            var enemy = Owner.RunState.Rng.CombatTargets.NextItem(CombatState!.HittableEnemies);
            if (enemy == null)
                break;
            await CardCmd.AutoPlay(ctx, strike, enemy);
        }
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Uncommon
 *   usings updated
 *   CanonicalVars removed → With* calls in constructor
 *   AdditionalHoverTips/ExtraHoverTips removed (covered by WithPower)
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(9, 3)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 */