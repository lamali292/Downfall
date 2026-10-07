using MegaCrit.Sts2.Core.Nodes.Ftue;

namespace Downfall.DownfallCode.Utils.UI.Ftue;

// Full-screen host registered as the single NModalContainer modal so several DownfallFtue popups
// (e.g. two pointer tips) can be shown together: each dismisses on its own, and only once every
// child popup it was told about has dismissed does this closes the modal for real.
public partial class NDownfallComboFtue : NFtue
{
    private int _remaining;

    public void Prepare(int count) => _remaining = count;

    public void Release()
    {
        if (--_remaining <= 0) CloseFtue();
    }
}
