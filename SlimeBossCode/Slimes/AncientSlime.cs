using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class AncientSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Specialist;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "hit");
    }

    public override async Task Command(PlayerChoiceContext ctx)
    {
        await DamageCmd.Attack(3)
            .FromSlime(this)
            .TargetingAllOpponents(CombatState)
            .Execute(ctx);
    }

    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (!Creature.IsAlive || player != Creature.PetOwner) return count;
        var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, 1, out _, this);
        return count + modified;
    }
}