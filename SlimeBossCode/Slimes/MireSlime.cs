using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class MireSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Normal;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "hit");
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        var enemy = CombatState.HittableEnemies.TakeRandom(1, CombatState.RunState.Rng.CombatTargets)
            .FirstOrDefault();
        if (enemy == null) return;
        await DamageCmd.Attack(2).FromSlime(this).Targeting(enemy).Execute(ctx);
        var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, 2, out _, this);
        await PowerCmd.Apply<GoopPower>(ctx, enemy, modified, PetOwner, null);
    }
}