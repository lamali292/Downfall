using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using SlimeBoss.SlimeBossCode.Extensions;

namespace SlimeBoss.SlimeBossCode.Slimes;

public class TorchheadSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.Specialist;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(6, DamageProps.monsterMove)];

    public override IEnumerable<IHoverTip> ExtraTips =>
    [
        HoverTipFactory.FromPower<StrengthPower>()
    ];
    
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("torchhead"));
        skeleton.SetSlotsToSetupPose();
    }


    public override async Task Command(PlayerChoiceContext ctx)
    {
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue + PetOwner.GetPowerAmount<StrengthPower>())
            .FromSlime(this)
            .TargetingRandomOpponents(CombatState)
            .Execute(ctx);
    }
}