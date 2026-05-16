using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class BronzeSlime : SlimeModel
{

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
        => SetupAnimationState(controller, "idle" , hitName: "hit");
}