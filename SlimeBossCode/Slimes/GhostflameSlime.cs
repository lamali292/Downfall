using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class GhostflameSlime : SlimeModel
{
    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "damage");
    }


    public override async Task Command(PlayerChoiceContext ctx)
    {
        var enemy = CombatState.HittableEnemies.TakeRandom(1, CombatState.RunState.Rng.CombatTargets)
            .FirstOrDefault();
        var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, 6, out _, this);
        if (enemy == null) return;
        await DamageCmd.Attack(4).FromSlime(this).Targeting(enemy).Execute(ctx);
        await PowerCmd.Apply<SoulBurnPower>(ctx, enemy, modified, Creature, null);
    }
}