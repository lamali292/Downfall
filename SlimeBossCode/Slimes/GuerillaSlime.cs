using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class GuerillaSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Normal;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "damage");
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3, ValueProp.Move)
    ];

    
    public override async Task Command(PlayerChoiceContext ctx)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this).TargetingAllOpponents(CombatState).Execute(ctx);
    }
}