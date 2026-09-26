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
        new DamageVar(3, DamageProps.nonCardUnpowered),
        new RepeatVar(2)
    ];

    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("attack"));
        skeleton.SetSlotsToSetupPose();
    }

    
    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this)
            .WithHitCount(DynamicVars.Repeat.IntValue);
        // "Mafioso - Bruiser Slime hits ALL enemies."
        attack = forcedTarget != null
            ? attack.Targeting(forcedTarget)
            : PetOwner.HasPower<MafiosoPower>()
                ? attack.TargetingAllOpponents(CombatState)
                : attack.TargetingRandomOpponents(CombatState);
        await attack.Execute(ctx);
    }
}