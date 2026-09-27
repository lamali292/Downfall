using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class GuerillaSlime : SlimeModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3, DamageProps.nonCardUnpowered)
    ];

    
    protected override string? SkinName => "poison";


    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this).TargetingAllOpponents(CombatState).Execute(ctx);
    }
}