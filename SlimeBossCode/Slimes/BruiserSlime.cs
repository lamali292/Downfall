using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class BruiserSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Normal;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3, DamageProps.nonCardUnpowered)
    ];

    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("attack"));
        skeleton.SetSlotsToSetupPose();
    }

    // "Deals damage to the highest HP enemy."
    private Creature? GetHighestHpOpponent()
    {
        return CombatState.GetOpponentsOf(Creature).Where(e => e.IsAlive).MaxBy(e => e.CurrentHp);
    }

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this);
        // "Mafioso - Bruiser Slime hits ALL enemies."
        var target = forcedTarget ?? GetHighestHpOpponent();
        attack = forcedTarget == null && PetOwner.HasPower<MafiosoPower>()
            ? attack.TargetingAllOpponents(CombatState)
            : target != null
                ? attack.Targeting(target)
                : attack.TargetingRandomOpponents(CombatState);
        await attack.Execute(ctx);
    }
}