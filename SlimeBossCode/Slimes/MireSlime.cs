using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.DynamicVars;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class MireSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Normal;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(2, DamageProps.monsterMove),
        new SlimeSecondaryVar(2)
    ];

    public override IEnumerable<IHoverTip> ExtraTips =>
    [
        HoverTipFactory.FromPower<GoopPower>()
    ];

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "hit");
    }


    public override async Task Command(PlayerChoiceContext ctx)
    {
        var cmd = await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this)
            .TargetingRandomOpponents(CombatState).Execute(ctx);
        var target = cmd.Results.SelectMany(e => e).Select(e => e.Receiver);
        var original = DynamicVars.Slime().IntValue;
        var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, original, out _, this);
        await PowerCmd.Apply<GoopPower>(ctx, target, modified, PetOwner, null);
    }
}