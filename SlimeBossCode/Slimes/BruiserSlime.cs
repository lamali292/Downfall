using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class BruiserSlime : SlimeModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3, DamageProps.nonCardUnpowered)
    ];

    protected override string? SkinName => "attack";
    
    private Creature? GetHighestHpOpponent()
    {
        return CombatState.GetOpponentsOf(Creature).Where(e => e.IsHittable).MaxBy(e => e.CurrentHp);
    }

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this);
        var target = forcedTarget ?? GetHighestHpOpponent();
        if (target == null) return;
        attack = PetOwner.HasPower<MafiosoPower>()
            ? attack.TargetingAllOpponents(CombatState)
            : attack.Targeting(target);
        await attack.Execute(ctx);
    }
}