using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SlimeBoss.SlimeBossCode.Slimes;

[Obsolete]
public class ScrapSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.None;
    
    
    public override void SetupSkins(MegaSprite spine, MegaSkeleton skeleton)
    {
        skeleton.SetSkin(skeleton.GetData().FindSkin("scrap"));
        skeleton.SetSlotsToSetupPose();
    }

    
    public override Task Command(PlayerChoiceContext ctx, Creature? forcedTarget = null)
    {
        throw new Exception();
    }
}