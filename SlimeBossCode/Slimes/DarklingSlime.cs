using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class DarklingSlime : SlimeModel, IAfterCommand
{
    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(3, DamageProps.nonCardUnpowered)
    ];

    // todo : change so this slime only visually stacks

    public override SlimeType SlimeType => SlimeType.Counter;

    public override Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var attack = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this);
        attack = forcedTarget != null ? attack.Targeting(forcedTarget) : attack.TargetingRandomOpponents(CombatState);
        return attack.Execute(ctx);
    }
    
    public async Task AfterCommand(PlayerChoiceContext ctx, Player player, SlimeModel slime, CardModel? source, bool isAutomatic)
    {
        if (player.Creature != PetOwner || slime == this || slime is DarklingSlime || isAutomatic) return;
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromSlime(this)
            .TargetingRandomOpponents(CombatState).WithHitCount(SlimeAmount).Execute(ctx);
    }
}
