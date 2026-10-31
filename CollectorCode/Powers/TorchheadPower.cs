using BaseLib.Patches.Localization;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class TorchheadPower : CollectorPowerModel, IAddDumbVariablesToPowerDescription
{

    public TorchheadPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithTorchheadDamage(4);
    }
    
    public override bool ShouldPlayVfx => false;

    private Creature? _target;
    private bool _dealtDamageThisTurn;

    /// <summary>The enemy Torchhead attacks: the one its owner hit last, for as long as it is still alive.</summary>
    public Creature? LastHitTarget => _target is { IsAlive: true } ? _target : null;

    public void SetLastHitTarget(Creature target) => _target = target;

    /// <summary>At the end of the turn Torchhead only attacks if its owner dealt damage that turn and has a living target.</summary>
    public bool ShouldAutoAttack => _dealtDamageThisTurn && AttackTarget != null;

    /// <summary>An enemy marked as a priority target (lowest HP first) if there is one, otherwise the enemy last hit.</summary>
    public Creature? AttackTarget =>
        Owner.CombatState?.HittableEnemies
            .Where(enemy => enemy.Powers.Any(power => power is IPrioritizesTorchheadTarget))
            .OrderBy(enemy => enemy.CurrentHp)
            .FirstOrDefault() ?? LastHitTarget;

    public override async Task AfterDamageGiven(PlayerChoiceContext ctx, Creature? dealer, DamageResult result,
        ValueProp props, Creature target, CardModel? cardSource)
    {
        var ownerCreature = Owner.PetOwner?.Creature;
        if (ownerCreature == null || !props.IsPoweredAttack() || result.TotalDamage <= 0 ||
            target.Side == ownerCreature.Side) return;

        if (dealer == ownerCreature)
        {
            _target = target;
            _dealtDamageThisTurn = true;
        }
        else if (dealer == Owner)
        {
            _dealtDamageThisTurn = true;
        }

        await Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (Owner.PetOwner?.Creature is { } ownerCreature && participants.Contains(ownerCreature))
            _dealtDamageThisTurn = false;
        return Task.CompletedTask;
    }
    
    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        DynamicVars.TorchheadDamage.UpdatePowerPreview(this, CardPreviewMode.None, null, IsMutable);
        var shouldTargetAll = IsMutable && Owner.PetOwner != null && CollectorHook.ShouldTorchheadTargetAll(Owner.PetOwner, out _);
        description.Add("TorchheadTargetsAll", shouldTargetAll);
    }
    
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        var petOwner = Owner.PetOwner;
        var creature = petOwner?.Creature;
        if (petOwner == null || creature == null || !participants.Contains(creature)) return;
        if (ShouldAutoAttack)
            await TorchheadCmd.TorchheadAttack(petOwner, DynamicVars.TorchheadDamage.IntValue).ExecuteIfPresent(ctx);

        if (Owner.IsAlive)
            TorchheadCmd.RefreshTorchheadIntent(Owner);
    }
    
    public override Creature ModifyUnblockedDamageTarget(
        Creature target,
        decimal _,
        ValueProp props,
        Creature? __)
    {
        return target != Owner.PetOwner?.Creature || Owner.IsDead || !props.IsPoweredAttack() ? target : Owner;
    }

    /// <summary>This is so Osty won't receive powers while it is dead</summary>
    public override bool ShouldAllowHitting(Creature creature) => creature.IsAlive;

    public override bool ShouldCreatureBeRemovedFromCombatAfterDeath(Creature creature)
    {
        return creature != Owner;
    }

    public override bool ShouldPowerBeRemovedAfterOwnerDeath() => false;
    
}