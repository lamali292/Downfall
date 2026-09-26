using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class RoyalSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Specialist;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, DamageProps.nonCardUnpowered)
    ];

    // NOTE: no scene/skin exists for this slime yet - reusing Bronze's "bronze" skin as a placeholder.
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("bronze"));
        skeleton.SetSlotsToSetupPose();
    }

    // Royal Slime does nothing on the normal start-of-turn Command; its attack happens at end of turn instead.
    public override Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (PetOwner.Player == null || !participants.Contains(PetOwner)) return;
        var hits = PetOwner.Player.SlimeCount;
        for (var i = 0; i < hits; i++)
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this)
                .TargetingRandomOpponents(CombatState).Execute(ctx);
    }
}
