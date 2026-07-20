using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.DynamicVars;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Powers;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class SpikySlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Specialist;

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return SetupAnimationState(controller, "idle", hitName: "damage");
    }

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new DamageVar(4, ValueProp.Move),
        new SlimeSecondaryVar(4)
    ];

    

    public override async Task Command(PlayerChoiceContext ctx)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromSlime(this).TargetingRandomOpponents(CombatState).Execute(ctx);
        var original = DynamicVars.Slime().IntValue;
        var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, original, out _, this);
        await PowerCmd.Apply<SpikySlimePower>(ctx, PetOwner, modified, Creature, null);
    }
    
    public override IEnumerable<IHoverTip> ExtraTips =>
    [
        HoverTipFactory.FromPower<ThornsPower>()
    ];
}

public class SpikySlimePower : CustomTemporaryPowerModelWrapper<SpikySlime, ThornsPower>
{
    protected override bool UntilEndOfOtherSideTurn => true;
}