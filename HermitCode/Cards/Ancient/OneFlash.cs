using BaseLib.Utils;
using Hermit.HermitCode.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hermit.HermitCode.Cards.Ancient;

/// <summary>
///     Deal 12 damage. Gain Block equal to unblocked damage dealt first.
///     Dead On: Gain 4 Strength.
///     Upgrade: 16 damage, gain 5 Strength.
/// </summary>
public sealed class OneFlash : HermitCardModel, IHasDeadOnEffect
{
    public OneFlash() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(12, 4);
        WithPower<StrengthPower>(4, 1);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Attack", Owner.Character.AttackAnimDelay);
        HermitSfx.PlayGun1();

        var runState = IRunState.GetFrom([play.Target!, Owner.Creature]);
        var modifiedDamage = Hook.ModifyDamage(runState, CombatState, play.Target!, Owner.Creature,
            DynamicVars.Damage.BaseValue, ValueProp.Move, this, ModifyDamageHookType.All, CardPreviewMode.None, out _);
        var unblockedDamage = Math.Max(0, (int)(modifiedDamage - play.Target!.Block));
        await CreatureCmd.GainBlock(Owner.Creature, unblockedDamage, ValueProp.Move, play);

        await CommonActions.CardAttack(this, play)
            .WithHermitGunHitFx()
            .Execute(ctx);
    }

    public async Task DeadOnEffect(PlayerChoiceContext ctx, CardPlay play)
    {
        await CommonActions.ApplySelf<StrengthPower>(ctx, this);
    }
}

/* transform_cards.py changes:
 *   namespace → Hermit.HermitCode.Cards.Ancient
 *   usings updated
 *   OnUpgrade removed (all logic migrated to constructor)
 *   constructor: WithDamage(12, 4), WithPower<StrengthPower>(4, 1)
 *   DamageCmd.Attack chain → CommonActions.CardAttack
 *   PowerCmd.Apply self → CommonActions.ApplySelf
 */