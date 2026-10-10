using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.DynamicVars;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class LeechingSlime : SlimeModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3, DamageProps.nonCardUnpowered),
        new PowerVar<WeakPower>(1)
    ];

    public override IEnumerable<IHoverTip> ExtraTips =>
    [
        HoverTipFactory.FromPower<WeakPower>()
    ];

    protected override string? SkinName => "sliming";

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this);
        attack = forcedTarget != null ? attack.Targeting(forcedTarget) : attack.TargetingRandomOpponents(CombatState);
        var cmd = await attack.Execute(ctx);
        
        var target = cmd.Results.SelectMany(e => e).Select(e => e.Receiver);
        await PowerCmd.Apply<WeakPower>(ctx, target, DynamicVars.Weak.IntValue, Creature, null);
    }
}
