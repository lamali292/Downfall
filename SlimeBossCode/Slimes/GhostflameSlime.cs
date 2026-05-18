using Downfall.DownfallCode.Utils;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
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
        if (enemy == null) return;
        await DamageCmd.Attack(4).FromSlime(this).Targeting(enemy).Execute(ctx);
        await ModCompat.TryExecute("Hexaghost",
            () => PowerCmd.Apply<SoulBurnPower>(ctx, enemy, 6,
                Creature, null)
        );
    }
}