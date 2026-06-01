using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class CultistSlime : SlimeModel
{
    private int _additionalDamage;

    public override SlimeType SlimeType => SlimeType.Specialist;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "damage");
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        await DamageCmd.Attack(6 + _additionalDamage)
            .FromSlime(this)
            .TargetingRandomOpponents(CombatState)
            .Execute(ctx);
        _additionalDamage++;
    }
}