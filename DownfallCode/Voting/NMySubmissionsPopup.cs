using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Self-service view of the current player's own art submissions (status,
/// upvotes, withdraw). Needs nothing from a caller - openable from any
/// screen via <see cref="OpenFrom"/>.
/// </summary>
public partial class NMySubmissionsPopup : Control
{
    public const string ScenePath = "res://Downfall/scenes/voting/my_submissions_popup.tscn";

    private static NMySubmissionsPopup? _current;

    public static void OpenFrom(Node parent)
    {
        if (GodotObject.IsInstanceValid(_current))
            return;

        var popup = GD.Load<PackedScene>(ScenePath).Instantiate<NMySubmissionsPopup>();
        _current = popup;
        popup.TreeExiting += () => _current = null;
        parent.AddChild(popup);
        TaskHelper.RunSafely(popup.Load());
    }

    private ColorRect _dim = null!;
    private Label _title = null!;
    private Label _status = null!;
    private VBoxContainer _list = null!;
    private Button _closeButton = null!;
    private Label _creditNameLabel = null!;
    private LineEdit _creditNameEdit = null!;
    private Button _saveCreditNameButton = null!;
    private Label _creditNameStatus = null!;

    public override void _Ready()
    {
        _dim = GetNode<ColorRect>("%Dim");
        _title = GetNode<Label>("%Title");
        _status = GetNode<Label>("%Status");
        _list = GetNode<VBoxContainer>("%List");
        _closeButton = GetNode<Button>("%CloseButton");
        _creditNameLabel = GetNode<Label>("%CreditNameLabel");
        _creditNameEdit = GetNode<LineEdit>("%CreditNameEdit");
        _saveCreditNameButton = GetNode<Button>("%SaveCreditNameButton");
        _creditNameStatus = GetNode<Label>("%CreditNameStatus");

        _title.Text = VotingUi.Loc("DOWNFALL-VOTING.my_submissions_button");
        _creditNameLabel.Text = VotingUi.Loc("DOWNFALL-VOTING.credit_name_label");
        _creditNameEdit.TooltipText = VotingUi.Loc("DOWNFALL-VOTING.credit_name_hint");
        VotingUi.StyleActionButton(_closeButton, primary: false);
        VotingUi.StyleActionButton(_saveCreditNameButton, primary: false);
        _closeButton.Text = VotingUi.Loc("DOWNFALL-VOTING.close_button");
        _saveCreditNameButton.Text = VotingUi.Loc("DOWNFALL-VOTING.save_credit_name_button");

        _dim.GuiInput += e =>
        {
            if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
                QueueFree();
        };
        _closeButton.Pressed += QueueFree;
        _saveCreditNameButton.Pressed += () => TaskHelper.RunSafely(SaveCreditName());
    }

    private async Task SaveCreditName()
    {
        var creditName = _creditNameEdit.Text.Trim();
        if (creditName.Length == 0)
            return;

        _saveCreditNameButton.Disabled = true;
        _creditNameStatus.Text = "";

        var (ok, error) = await VotingApi.Instance.SetMyCreditName(creditName);
        if (!IsInstanceValid(this))
            return;

        _saveCreditNameButton.Disabled = false;
        _creditNameStatus.Text = ok
            ? VotingUi.Loc("DOWNFALL-VOTING.status_credit_name_saved")
            : error ?? VotingUi.Loc("DOWNFALL-VOTING.error_credit_name_save_failed");
    }

    private async Task Load()
    {
        if (!VotingAuth.IsSignedIn)
        {
            _status.Text = VotingUi.Loc("DOWNFALL-VOTING.status_signing_in");
            var (ok, message) = await VotingAuth.LoginAsync();
            if (!IsInstanceValid(this))
                return;
            if (!ok)
            {
                _status.Text = message;
                return;
            }
        }

        var saved = await VotingApi.Instance.GetMyCreditName();
        if (!IsInstanceValid(this))
            return;
        if (!string.IsNullOrEmpty(saved))
            _creditNameEdit.Text = saved;

        await Refresh();
    }

    private async Task Refresh()
    {
        _status.Text = VotingUi.Loc("DOWNFALL-VOTING.status_loading");
        var submissions = await VotingApi.Instance.GetMySubmissions();
        if (!IsInstanceValid(this))
            return;

        foreach (var child in _list.GetChildren())
            child.QueueFree();

        if (submissions == null)
        {
            _status.Text = VotingUi.Loc("DOWNFALL-VOTING.error_load_failed");
            return;
        }

        if (submissions.Count == 0)
        {
            _status.Text = VotingUi.Loc("DOWNFALL-VOTING.status_empty");
            return;
        }

        _status.Text = VotingUi.Loc("DOWNFALL-VOTING.status_count", ("count", submissions.Count.ToString()));

        foreach (var sub in submissions)
            BuildRow(sub);
    }

    private void BuildRow(MySubmission sub)
    {
        var font = GD.Load<FontVariation>("res://themes/kreon_regular_shared.tres");
        var cream = new Color(1f, 0.964706f, 0.886275f);

        var row = new PanelContainer();
        row.AddChild(new ColorRect { Color = new Color(0.1f, 0.13f, 0.15f, 0.6f) });

        var margin = new MarginContainer();
        margin.AddThemeConstantOverride("margin_left", 10);
        margin.AddThemeConstantOverride("margin_top", 10);
        margin.AddThemeConstantOverride("margin_right", 10);
        margin.AddThemeConstantOverride("margin_bottom", 10);
        row.AddChild(margin);

        var hbox = new HBoxContainer();
        hbox.AddThemeConstantOverride("separation", 12);
        margin.AddChild(hbox);

        var thumbFrame = new PanelContainer { CustomMinimumSize = new Vector2(80, 80) };
        thumbFrame.AddChild(new ColorRect { Color = new Color(0.05f, 0.07f, 0.08f) });
        if (sub.ImagePath != null)
        {
            var thumb = new TextureRect
            {
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            };
            thumb.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            thumbFrame.AddChild(thumb);
            _ = LoadThumbnail(thumb, sub.ImagePath);
        }
        hbox.AddChild(thumbFrame);

        var infoBox = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        hbox.AddChild(infoBox);

        infoBox.AddChild(VotingUi.ThemedLabel(sub.Card?.Title ?? sub.ModelId.Entry, font, cream, 20));

        var (statusText, statusColor) = sub.Status switch
        {
            "approved" => (VotingUi.Loc("DOWNFALL-VOTING.status_approved", ("votes", sub.Upvotes.ToString())), new Color(0.5f, 0.9f, 0.5f)),
            "rejected" => (VotingUi.Loc("DOWNFALL-VOTING.status_rejected"), new Color(0.9f, 0.4f, 0.4f)),
            _ => (VotingUi.Loc("DOWNFALL-VOTING.status_pending"), new Color(0.9f, 0.8f, 0.4f)),
        };
        infoBox.AddChild(VotingUi.ThemedLabel(statusText, font, statusColor, 16));

        var withdrawButton = VotingUi.CreateActionButton(VotingUi.Loc("DOWNFALL-VOTING.withdraw_button"), primary: false);
        hbox.AddChild(withdrawButton);

        _list.AddChild(row);
        withdrawButton.Pressed += () => TaskHelper.RunSafely(Withdraw(sub, withdrawButton));
    }

    private async Task Withdraw(MySubmission sub, Button withdrawButton)
    {
        withdrawButton.Disabled = true;
        var ok = await VotingApi.Instance.DeleteMySubmission(sub.Id);
        if (!IsInstanceValid(this))
            return;
        if (!ok)
        {
            withdrawButton.Disabled = false;
            _status.Text = VotingUi.Loc("DOWNFALL-VOTING.error_withdraw_failed");
            return;
        }
        await Refresh();
    }

    private async Task LoadThumbnail(TextureRect thumb, string url)
    {
        if (NVoteCard.TextureCache.TryGetValue(url, out var cached))
        {
            thumb.Texture = cached;
            return;
        }

        var http = new HttpRequest();
        AddChild(http);

        if (http.Request(url) != Error.Ok)
        {
            http.QueueFree();
            return;
        }

        var result = await ToSignal(http, HttpRequest.SignalName.RequestCompleted);
        http.QueueFree();

        if (!IsInstanceValid(thumb))
            return;

        var body = result[3].AsByteArray();
        var img = new Image();

        if (img.LoadPngFromBuffer(body) != Error.Ok &&
            img.LoadJpgFromBuffer(body) != Error.Ok &&
            img.LoadWebpFromBuffer(body) != Error.Ok)
        {
            return;
        }

        var tex = ImageTexture.CreateFromImage(img);
        NVoteCard.TextureCache[url] = tex;
        thumb.Texture = tex;
    }
}
