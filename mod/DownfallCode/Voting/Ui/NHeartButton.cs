using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Downfall.DownfallCode.Voting;

public partial class NHeartButton : NButton
{
    protected override void OnRelease()
    {
        Pressed?.Invoke();
    }
    
    public override void _Ready()
    {
        this.ConnectSignals();
    }

    public Action? Pressed { get; set; }
}