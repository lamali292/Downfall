namespace Downfall.DownfallCode.Voting;

using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

public partial class NVoteCard : PanelContainer
{
    internal static readonly Dictionary<string, Texture2D> TextureCache = new();

    private static readonly (string reason, string label)[] ReportReasons =
    {
        ("ai", "AI-generated"),
        ("stolen", "Stolen / copyright"),
        ("inappropriate", "NSFW / inappropriate"),
        ("offtopic", "Off-topic"),
        ("other", "Other"),
    };

    private TextureRect _image = null!;
    private Label _authorLabel = null!;
    private Label _count = null!;
    private Button _upButton = null!;
    private Button _downButton = null!;
    private Button _reportButton = null!;

    private string _imagePath = "";
    private ArtEntry? _pending;
    private long _submissionId;
    private int _up;
    private int _down;
    private int _myVote;
    private readonly HashSet<string> _myFlags = new();

    public int Score => _up - _down;

    [Signal] public delegate void ScoreChangedEventHandler();
    [Signal] public delegate void CardClickedEventHandler(string imagePath);

    public override void _Ready()
    {
        _image        = GetNode<TextureRect>("MarginContainer/VBoxContainer/Image");
        _authorLabel  = GetNode<Label>("MarginContainer/VBoxContainer/AuthorLabel");
        _upButton     = GetNode<Button>("MarginContainer/VBoxContainer/VoteRow/UpButton");
        _count        = GetNode<Label>("MarginContainer/VBoxContainer/VoteRow/CountLabel");
        _downButton   = GetNode<Button>("MarginContainer/VBoxContainer/VoteRow/DownButton");
        _reportButton = GetNode<Button>("MarginContainer/VBoxContainer/VoteRow/ReportButton");

        _upButton.Pressed     += () => Vote(1);
        _downButton.Pressed   += () => Vote(-1);
        _reportButton.Pressed += OpenReportPopup;

        _image.GuiInput += OnImageGuiInput;
        _image.MouseFilter = MouseFilterEnum.Stop;

        if (_pending != null)
            Apply(_pending);
    }

    private void OnImageGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
            EmitSignal(SignalName.CardClicked, _imagePath);
    }

    public void SetEntry(ArtEntry entry)
    {
        _pending = entry;
        if (IsNodeReady())
            Apply(entry);
    }

    private void Apply(ArtEntry entry)
    {
        _submissionId = entry.Id;
        _authorLabel.Text = entry.Author;
        _imagePath = entry.ImagePath;
        _up = entry.Upvotes;
        _down = entry.Downvotes;
        _myVote = entry.MyVote;

        _myFlags.Clear();
        foreach (var f in entry.MyFlags)
            _myFlags.Add(f);

        Refresh();
        UpdateVoteHighlight();
        UpdateReportHighlight();
        _ = LoadImageAsync(entry.ImagePath);
    }

    private void Vote(int value)
    {
        // schon dieser Vote aktiv? → ausschalten (zurück auf neutral)
        var newVote = _myVote == value ? 0 : value;

        // alten Vote rückgängig
        switch (_myVote)
        {
            case 1:  _up--;   break;
            case -1: _down--; break;
        }

        _myVote = newVote;

        // neuen Vote anwenden (bei 0 passiert nichts)
        switch (_myVote)
        {
            case 1:  _up++;   break;
            case -1: _down++; break;
        }

        Refresh();
        UpdateVoteHighlight();

        _ = _myVote == 0 ? VotingApi.Instance.ClearVote(_submissionId) : VotingApi.Instance.CastVote(_submissionId, _myVote);
    }

    
    private void OpenReportPopup()
    {
        var draft = new HashSet<string>(_myFlags);

        var popup = new PopupPanel();
        var vbox = new VBoxContainer();
        popup.AddChild(vbox);

        vbox.AddChild(new Label { Text = "Report this submission:" });

        foreach (var (reason, label) in ReportReasons)
        {
            var check = new CheckBox { Text = label, ButtonPressed = draft.Contains(reason) };
            var r = reason;
            check.Toggled += on =>
            {
                if (on) draft.Add(r); else draft.Remove(r);
            };
            vbox.AddChild(check);
        }

        var sendButton = new Button { Text = "Send report" };
        sendButton.Pressed += () =>
        {
            SubmitReport(draft);
            popup.Hide();
        };
        vbox.AddChild(sendButton);

        AddChild(popup);
        popup.PopupCentered();
        popup.PopupHide += () => popup.QueueFree();
    }

    private void SubmitReport(HashSet<string> draft)
    {
        foreach (var reason in draft)
            if (!_myFlags.Contains(reason))
                _ = VotingApi.Instance.ToggleFlag(_submissionId, reason, true);

        foreach (var reason in _myFlags)
            if (!draft.Contains(reason))
                _ = VotingApi.Instance.ToggleFlag(_submissionId, reason, false);

        _myFlags.Clear();
        foreach (var r in draft) _myFlags.Add(r);

        UpdateReportHighlight();
    }

    private void UpdateReportHighlight()
    {
        _reportButton.Modulate = _myFlags.Count > 0
            ? new Color(1f, 0.6f, 0.3f)
            : Colors.White;
    }

    private static readonly Color UpColor   = new(1f, 0.6f, 0.2f);   // orange
    private static readonly Color DownColor = new(0.3f, 0.55f, 1f);  // blau

    private void UpdateVoteHighlight()
    {
        _upButton.Modulate   = _myVote == 1  ? UpColor   : Colors.White;
        _downButton.Modulate = _myVote == -1 ? DownColor : Colors.White;
    }

    private async Task LoadImageAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (TextureCache.TryGetValue(path, out var cached))
        {
            _image.Texture = cached;
            return;
        }

        var tex = await ResolveTexture(path);
        if (tex == null || !IsInstanceValid(this))
            return;

        TextureCache[path] = tex;
        if (_imagePath == path)
            _image.Texture = tex;
    }

    private async Task<Texture2D?> ResolveTexture(string path)
    {
        if (path.StartsWith("res://"))
            return ResourceLoader.Exists(path) ? GD.Load<Texture2D>(path) : null;

        if (path.StartsWith("http://") || path.StartsWith("https://"))
            return await Download(path);

        if (!FileAccess.FileExists(path)) return null;
        var img = new Image();
        return img.Load(path) == Error.Ok ? ImageTexture.CreateFromImage(img) : null;
    }

    private async Task<Texture2D?> Download(string url)
    {
        var http = new HttpRequest();
        AddChild(http);
        if (http.Request(url) != Error.Ok) { http.QueueFree(); return null; }

        var result = await ToSignal(http, HttpRequest.SignalName.RequestCompleted);
        http.QueueFree();

        var body = result[3].AsByteArray();
        var img = new Image();
        if (img.LoadPngFromBuffer(body) != Error.Ok &&
            img.LoadJpgFromBuffer(body) != Error.Ok &&
            img.LoadWebpFromBuffer(body) != Error.Ok)
            return null;

        return ImageTexture.CreateFromImage(img);
    }

    private void Refresh()
    {
        _count.Text = (_up - _down).ToString();
        EmitSignal(SignalName.ScoreChanged);
    }
}