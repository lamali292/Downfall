using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class DrawingSlime : SlimeModel
{

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
        => SetupAnimationState(controller, "idle" , hitName: "hit");
}