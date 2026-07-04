using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class SpikySlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Specialist;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "damage");
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        await DamageCmd.Attack(4).FromSlime(this).TargetingRandomOpponents(CombatState).Execute(ctx);
        var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, 4, out _, this);
        await PowerCmd.Apply<SpikySlimePower>(ctx, PetOwner, modified, Creature, null);
    }
}

public class SpikySlimePower : CustomTemporaryPowerModelWrapper<SpikySlime, ThornsPower>
{
    protected override bool UntilEndOfOtherSideTurn => true;
}