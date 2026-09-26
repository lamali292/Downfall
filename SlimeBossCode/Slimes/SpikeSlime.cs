using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.DynamicVars;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class SpikeSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Specialist;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SlimeSecondaryVar(4)
    ];

    public override IEnumerable<IHoverTip> ExtraTips =>
    [
        HoverTipFactory.FromPower<ThornsPower>()
    ];
    
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("protector"));
        skeleton.SetSlotsToSetupPose();
    }
    


    // "Does not attack at the end of your turn. Instead, attacks enemies whenever you are attacked."
    public override Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var original = DynamicVars.Slime.IntValue;
        var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, original, out _, this);
        return PowerCmd.Apply<SpikeSlimePower>(ctx, PetOwner, modified, Creature, null);
    }
}

public class SpikeSlimePower : CustomTemporaryPowerModelWrapper<SpikeSlime, ThornsPower>
{
    protected override bool UntilEndOfOtherSideTurn => true;
}