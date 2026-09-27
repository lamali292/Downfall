using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class RoyalSlime : SlimeModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(7, DamageProps.nonCardUnpowered)
    ];

    protected override string? SkinName => "poison";

    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        if (PetOwner.Player == null) return;
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .WithHitCount(PetOwner.Player.SlimeCount)
            .FromSlime(this);
        attack = forcedTarget != null ? attack.Targeting(forcedTarget) : attack.TargetingRandomOpponents(CombatState);
        await attack.Execute(ctx);
    }
    
}
