using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class MassiveSlime : SlimeModel
{
    private int _skipTurns;
    
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(20, DamageProps.nonCardUnpowered),
        new("Sleep", 1)
    ];

    protected override string? SkinName => "poison";

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        if (_skipTurns > 0)
        {
            _skipTurns--;
            return;
        }
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromSlime(this)
            .TargetingAllOpponents(CombatState).Execute(ctx);
        _skipTurns = DynamicVars["Sleep"].IntValue;
    }
}
