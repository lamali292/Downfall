using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class DarklingSlime : SlimeModel
{
    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "Idle", hitName: "Hit", attackName: "Attack");
    }

    public override Task Command(PlayerChoiceContext ctx)
    {
        throw new NotImplementedException();
    }
}