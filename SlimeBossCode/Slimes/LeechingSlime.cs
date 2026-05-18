using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class LeechingSlime : SlimeModel
{
    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "damage");
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        await DamageCmd.Attack(1).FromSlime(this).TargetingRandomOpponents(CombatState).Execute(ctx);
        await CreatureCmd.GainBlock(PetOwner, 3, ValueProp.Move, null);
    }
}