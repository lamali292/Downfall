using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class MassiveSlime : SlimeModel
{
    private int _skipTurns;

    public override SlimeType SlimeType => SlimeType.Specialist;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20, DamageProps.nonCardUnpowered),
        new("Sleep", 1)
    ];

    // NOTE: no scene/skin exists for this slime yet - reusing Bronze's "bronze" skin as a placeholder.
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("bronze"));
        skeleton.SetSlotsToSetupPose();
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        if (_skipTurns > 0)
        {
            _skipTurns--;
            return;
        }

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromSlime(this)
            .TargetingAllOpponents(CombatState)
            .Execute(ctx);
        _skipTurns = DynamicVars["Sleep"].IntValue;
    }
}
