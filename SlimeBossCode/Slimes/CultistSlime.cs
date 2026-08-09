using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class CultistSlime : SlimeModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(6, DamageProps.monsterMove),
        new("Increase", 1)
    ];

    public override SlimeType SlimeType => SlimeType.Specialist;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "damage");
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromSlime(this)
            .TargetingRandomOpponents(CombatState)
            .Execute(ctx);
        DynamicVars.Damage.BaseValue += DynamicVars["Increase"].BaseValue;
    }
}