using Godot;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// A single selectable reason chip in <see cref="NReportPopup"/>'s reason
/// list. Owns its own visuals/state instead of being a bare <see cref="Button"/>
/// built inline by the popup, the same way <see cref="NHeartButton"/> owns
/// the vote button.
/// </summary>
public partial class NReportReasonToggle : Button
{
    [Signal]
    public delegate void ReasonToggledEventHandler(string reason, bool on);

    public string Reason { get; private set; } = "";

    public override void _Ready()
    {
        VotingUi.StyleToggleButton(this);
        Toggled += on => EmitSignal(SignalName.ReasonToggled, Reason, on);
    }

    public void Configure(string reason, string label, bool isOn)
    {
        Reason = reason;
        Text = label;
        SetPressedNoSignal(isOn);
    }
}
