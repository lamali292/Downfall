using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class SlimingSlime : SlimeModel
{
    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
        => SetupAnimationState(controller, "idle" , hitName: "hit");
 
}