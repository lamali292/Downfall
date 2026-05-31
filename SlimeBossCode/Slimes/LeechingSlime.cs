using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class LeechingSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Normal;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "damage");
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        await DamageCmd.Attack(1).FromSlime(this).TargetingRandomOpponents(CombatState).Execute(ctx);
        var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, 3, out _, this);
        await CreatureCmd.GainBlock(PetOwner, modified, ValueProp.Move, null);
    }
}