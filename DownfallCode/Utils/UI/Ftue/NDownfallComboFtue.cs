using MegaCrit.Sts2.Core.Nodes.Ftue;

namespace Downfall.DownfallCode.Utils.UI.Ftue;

// Full-screen host registered as the single NModalContainer modal so several DownfallFtue popups can
// be shown together on one screen without reopening the modal between them — either one at a time
// (DownfallFtue.ShowComboStep attaches the next popup as each one dismisses, then calls Finish()) or
// all at once (ShowComboSimultaneous attaches every popup up front via PrepareSimultaneous/ReleaseSimultaneous).
public partial class NDownfallComboFtue : NFtue
{
    private int _remaining;

    public void PrepareSimultaneous(int count) => _remaining = count;

    public void ReleaseSimultaneous()
    {
        if (--_remaining <= 0) CloseFtue();
    }

    public void Finish() => CloseFtue();
}
