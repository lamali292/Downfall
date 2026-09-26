using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class BronzeSlime : SlimeModel
{
    private int _skipTurns;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(10, DamageProps.nonCardUnpowered),
        new("Sleep", 2)
    ];

    public override SlimeType SlimeType => SlimeType.Specialist;
    
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("bronze"));
        skeleton.SetSlotsToSetupPose();
    }


    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        if (_skipTurns > 0)
        {
            _skipTurns--;
            return;
        }

        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this);
        attack = forcedTarget != null ? attack.Targeting(forcedTarget) : attack.TargetingAllOpponents(CombatState);
        await attack.Execute(ctx);
        _skipTurns = DynamicVars["Sleep"].IntValue;
    }
}