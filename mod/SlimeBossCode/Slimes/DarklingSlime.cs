using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace SlimeBoss.SlimeBossCode.Slimes;

[Obsolete]
public class DarklingSlime : SlimeModel
{
    public override SlimeType SlimeType => SlimeType.None;

    public override Task Command(PlayerChoiceContext ctx)
    {
        throw new Exception();
    }
}