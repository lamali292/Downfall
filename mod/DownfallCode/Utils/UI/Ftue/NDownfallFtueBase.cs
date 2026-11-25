using MegaCrit.Sts2.Core.Nodes.Ftue;

namespace Downfall.DownfallCode.Utils.UI.Ftue;

// Common base of our FTUE popups. Newer game builds made NFtue.SeenOnShowId abstract; a subclass without it fails to
// load, which makes Assembly.GetTypes() throw and takes the whole mod down. We compile against the newest game, and
// on older builds (no such member) this override simply becomes an unused virtual, so the type still loads.
public abstract partial class NDownfallFtueBase : NFtue
{
    // null = the game doesn't record these as "seen"; DownfallFtue tracks that itself.
    protected override string? SeenOnShowId => null;
}
