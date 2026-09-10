using BaseLib.Patches.Localization;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Events;
using Collector.CollectorCode.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.Powers;

public class TorchheadPower : CollectorPowerModel, IAddDumbVariablesToPowerDescription
{

    public TorchheadPower() : base(PowerType.Buff, PowerStackType.Single)
    {
        WithTorchheadDamage(5);
    }
    
    
    public override bool ShouldPlayVfx => false;

    
    public void AddDumbVariablesToPowerDescription(LocString description)
    {
        DynamicVars.TorchheadDamage.UpdatePowerPreview(this, CardPreviewMode.None, null, IsMutable);
        var shouldTargetAll = _owner?.PetOwner != null && CollectorHook.ShouldTorchheadTargetAll(_owner.PetOwner, out _);
        description.Add("TorchheadTargetsAll", shouldTargetAll);
    }
    
    
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side, IEnumerable<Creature> participants)
    {
        var petOwner = Owner.PetOwner;
        var creature = petOwner?.Creature;
        if (petOwner == null || creature == null || !participants.Contains(creature)) return;
        await CollectorCmd.TorchheadAttack(petOwner, DynamicVars.TorchheadDamage.IntValue).ExecuteIfPresent(ctx);

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