using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class InsultingSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Specialist;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "hit");
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        var enemy = CombatState.HittableEnemies.TakeRandom(1, CombatState.RunState.Rng.CombatTargets)
            .FirstOrDefault();
        if (enemy == null) return;
        await DamageCmd.Attack(4).FromSlime(this).Targeting(enemy).Execute(ctx);
        var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, 1, out _, this);
        await PowerCmd.Apply<VulnerablePower>(ctx, enemy, modified, Creature, null);
    }
}