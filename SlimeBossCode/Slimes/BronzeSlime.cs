using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class BronzeSlime : SlimeModel
{
    private bool _shouldSkip;

    public override SlimeType SlimeType => SlimeType.Specialist;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "hit");
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        if (_shouldSkip)
        {
            _shouldSkip = false;
            return;
        }

        await DamageCmd.Attack(10)
            .FromSlime(this)
            .TargetingAllOpponents(CombatState)
            .Execute(ctx);
        _shouldSkip = true;
    }
}