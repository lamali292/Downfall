using Godot;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Report-reason picker for a submission, styled like the rest of the
/// voting popups (<see cref="NUploadArtPopup"/>, <see cref="NSelectCardPopup"/>)
/// instead of the raw <see cref="PopupPanel"/>/<see cref="CheckBox"/> it used
/// to be built from inline in <see cref="NVoteCard"/>.
/// </summary>
public partial class NReportPopup : Control
{
    public const string ScenePath = "res://Downfall/scenes/voting/report_popup.tscn";

    private const string ReasonToggleScenePath = "res://Downfall/scenes/voting/report_reason_toggle.tscn";

    private static readonly (string reason, string locKey)[] ReportReasons =
    {
        ("ai", "DOWNFALL-VOTING.report_reason_ai"),
        ("stolen", "DOWNFALL-VOTING.report_reason_stolen"),
        ("inappropriate", "DOWNFALL-VOTING.report_reason_inappropriate"),
        ("offtopic", "DOWNFALL-VOTING.report_reason_offtopic"),
        ("other", "DOWNFALL-VOTING.report_reason_other"),
    };

    private static NReportPopup? _current;

    public static void OpenFrom(Node parent, IEnumerable<string> currentFlags, Action<HashSet<string>> onSubmit)
    {
        if (GodotObject.IsInstanceValid(_current))
            return;

        var popup = GD.Load<PackedScene>(ScenePath).Instantiate<NReportPopup>();
        _current = popup;
        popup.TreeExiting += () => _current = null;
        popup._draft.UnionWith(currentFlags);
        popup._onSubmit = onSubmit;
        parent.AddChild(popup);
    }

    private ColorRect _dim = null!;
    private Label _title = null!;
    private Label _subtitle = null!;
    private VBoxContainer _reasonList = null!;
    private Button _cancelButton = null!;
    private Button _sendButton = null!;

    private readonly HashSet<string> _draft = new();
    private Action<HashSet<string>>? _onSubmit;

    public override void _Ready()
    {
        _dim = GetNode<ColorRect>("%Dim");
        _title = GetNode<Label>("%Title");
        _subtitle = GetNode<Label>("%Subtitle");
        _reasonList = GetNode<VBoxContainer>("%ReasonList");
        _cancelButton = GetNode<Button>("%CancelButton");
        _sendButton = GetNode<Button>("%SendButton");

        _title.Text = VotingUi.Loc("DOWNFALL-VOTING.report_title");
        _subtitle.Text = VotingUi.Loc("DOWNFALL-VOTING.report_subtitle");

        VotingUi.StyleActionButton(_cancelButton, primary: false);
        VotingUi.StyleActionButton(_sendButton, primary: true);
        _cancelButton.Text = VotingUi.Loc("DOWNFALL-VOTING.cancel_button");
        _sendButton.Text = VotingUi.Loc("DOWNFALL-VOTING.send_report_button");

        _dim.GuiInput += e =>
        {
            if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
                QueueFree();
        };
        _cancelButton.Pressed += QueueFree;
        _sendButton.Pressed += () =>
        {
            _onSubmit?.Invoke(_draft);
            QueueFree();
        };

        var toggleScene = GD.Load<PackedScene>(ReasonToggleScenePath);
        foreach (var (reason, locKey) in ReportReasons)
        {
            var toggle = toggleScene.Instantiate<NReportReasonToggle>();
            toggle.Configure(reason, VotingUi.Loc(locKey), _draft.Contains(reason));
            toggle.ReasonToggled += (r, on) =>
            {
                if (on)
                    _draft.Add(r);
                else
                    _draft.Remove(r);
            };

            _reasonList.AddChild(toggle);
        }
    }
}
