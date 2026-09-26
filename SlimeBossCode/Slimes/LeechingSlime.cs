using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.DynamicVars;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class LeechingSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Normal;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new SlimeSecondaryVar(4)
    ];

    public override IEnumerable<IHoverTip> ExtraTips =>
    [
        HoverTipFactory.Static(StaticHoverTip.Block)
    ];
    

    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("shield"));
        skeleton.SetSlotsToSetupPose();
    }

    
    // "Grants Block instead of dealing damage."
    public override async Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        var original = DynamicVars.Slime.IntValue;
        var modified = SlimeBossHook.ModifySecondarySlimeEffects(CombatState, original, out _, this);
        await CreatureCmd.GainBlock(PetOwner, modified, BlockProps.nonCardUnpowered, null);
    }
}