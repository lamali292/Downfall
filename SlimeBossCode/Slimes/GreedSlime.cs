using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SlimeBoss.SlimeBossCode.Slimes;

[Obsolete]
public class GreedSlime : SlimeModel
{
    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "damage");
    }

    public override bool IsSpecialist => false;
    
    public override Task Command(PlayerChoiceContext ctx)
    {
        throw new NotImplementedException();
    }
}