using Godot;
using MegaCrit.Sts2.Core.Models;
using FileAccess = Godot.FileAccess;

namespace Downfall.DownfallCode.Voting;

public partial class NVoteCard : PanelContainer
{
    [Signal]
    public delegate void CardClickedEventHandler(string imagePath, string category,  string entry);

    [Signal]
    public delegate void ScoreChangedEventHandler();

    internal static readonly Dictionary<string, Texture2D> TextureCache = new();

    private static readonly Dictionary<VotingPool, string> IconPaths = new()
    {
        [VotingPool.Automaton] = "res://Automaton/images/character/character_icon.png",
        [VotingPool.Awakened]  = "res://Awakened/images/character/character_icon.png",
        [VotingPool.Champ]     = "res://Champ/images/character/character_icon.png",
        [VotingPool.Guardian]  = "res://Guardian/images/character/character_icon.png",
        [VotingPool.Hermit]    = "res://Hermit/images/character/character_icon.png",
        [VotingPool.Hexaghost] = "res://Hexaghost/images/character/character_icon.png",
        [VotingPool.Slimeboss] = "res://SlimeBoss/images/character/character_icon.png",
        [VotingPool.Snecko]    = "res://Snecko/images/character/character_icon.png",
    };

    private static readonly Dictionary<VotingPool, Texture2D> IconCache = new();

    private static readonly (string reason, string label)[] ReportReasons =
    {
        ("ai", "AI-generated"),
        ("stolen", "Stolen / copyright"),
        ("inappropriate", "NSFW / inappropriate"),
        ("offtopic", "Off-topic"),
        ("other", "Other")
    };

    private static readonly Color UpColor = new(1f, 0.2f, 0.2f);
    private readonly HashSet<string> _myFlags = new();

    private TextureRect _image = null!;
    private TextureRect _characterIcon = null!;
    private Label _cardLabel = null!;
    private Label _authorLabel = null!;
    private Label _count = null!;
    private NHeartButton _upButton = null!;
    private Button _reportButton = null!;

    private string _imagePath = "";
    private int _up;
    private bool _liked;
    private long _submissionId;
    private VotingPool _pool;
    private ArtEntry? _pending;

    // ---- Data the filter/sort reads ----

    public ModelId ModelId { get; set; }
    public string CardName { get; private set; } = "";
    public string Author { get; private set; } = "";
    public int Likes  => _up;
    public long SubmittedAt { get; private set; }

    /// <summary>
    /// Which character pool this card belongs to. Set by the screen from the
    /// category (ArtData), since a submission itself carries no character.
    /// Updating it refreshes the icon.
    /// </summary>
    public VotingPool Pool
    {
        get => _pool;
        set
        {
            _pool = value;
            if (IsNodeReady())
                UpdateCharacterIcon();
        }
    }

    public override void _Ready()
    {
        _image = GetNode<TextureRect>("%Image");
        _characterIcon = GetNode<TextureRect>("%CharacterIcon");
        _cardLabel = GetNode<Label>("%CardLabel");
        _authorLabel = GetNode<Label>("%AuthorLabel");
        _upButton = GetNode<NHeartButton>("%UpButton");
        _count = GetNode<Label>("%CountLabel");
        _reportButton = GetNode<Button>("%ReportButton");

        _upButton.Pressed += Like;
        _reportButton.Pressed += OpenReportPopup;

        _image.GuiInput += OnImageGuiInput;
        _image.MouseFilter = MouseFilterEnum.Stop;

        UpdateCharacterIcon();

        if (_pending != null)
            Apply(_pending);
    }

    private void OnImageGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton
            {
                Pressed: true,
                ButtonIndex: MouseButton.Left
            })
        {
            EmitSignal(SignalName.CardClicked, _imagePath, ModelId.Category, ModelId.Entry);
        }
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
        _imagePath = entry.ImagePath;
        _up = entry.Upvotes;
        
        _liked = entry.Liked;
        ModelId = entry.ModelId;
        CardName = entry.Card?.Title ?? "";
        Author = entry.Author;
        SubmittedAt = entry.SubmittedAt;

        _cardLabel.Text =  CardName;
        _authorLabel.Text = Author;

        _myFlags.Clear();
        foreach (var flag in entry.MyFlags)
            _myFlags.Add(flag);

        Refresh();
        UpdateVoteHighlight();
        UpdateReportHighlight();

        _ = LoadImageAsync(entry.ImagePath);
    }

 

    private void UpdateCharacterIcon()
    {
        if (!IconCache.TryGetValue(_pool, out var tex))
        {
            var path = IconPaths[_pool];
            tex = ResourceLoader.Exists(path)
                ? GD.Load<Texture2D>(path)
                : null;

            if (tex != null)
                IconCache[_pool] = tex;
        }

        if (tex != null)
            _characterIcon.Texture = tex;
    }

    private void Like()
    {
        _liked = !_liked;
        _up += _liked ? 1 : -1;

        Refresh();
        UpdateVoteHighlight();

        _ = _liked
            ? VotingApi.Instance.CastVote(_submissionId)
            : VotingApi.Instance.ClearVote(_submissionId);
    }

    private void UpdateVoteHighlight()
    {
        _upButton.Modulate = _liked
            ? UpColor
            : Colors.White;
    }

    private void OpenReportPopup()
    {
        var draft = new HashSet<string>(_myFlags);

        var popup = new PopupPanel();
        var vbox = new VBoxContainer();
        popup.AddChild(vbox);

        vbox.AddChild(new Label
        {
            Text = "Report this submission:"
        });

        foreach (var (reason, label) in ReportReasons)
        {
            var check = new CheckBox
            {
                Text = label,
                ButtonPressed = draft.Contains(reason)
            };

            var r = reason;

            check.Toggled += on =>
            {
                if (on)
                    draft.Add(r);
                else
                    draft.Remove(r);
            };

            vbox.AddChild(check);
        }

        var sendButton = new Button
        {
            Text = "Send report"
        };

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
        {
            if (!_myFlags.Contains(reason))
                _ = VotingApi.Instance.ToggleFlag(_submissionId, reason, true);
        }

        foreach (var reason in _myFlags)
        {
            if (!draft.Contains(reason))
                _ = VotingApi.Instance.ToggleFlag(_submissionId, reason, false);
        }

        _myFlags.Clear();

        foreach (var reason in draft)
            _myFlags.Add(reason);

        UpdateReportHighlight();
    }

    private void UpdateReportHighlight()
    {
        _reportButton.Modulate = _myFlags.Count > 0
            ? new Color(1f, 0.6f, 0.3f)
            : Colors.White;
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
        {
            return ResourceLoader.Exists(path)
                ? GD.Load<Texture2D>(path)
                : null;
        }

        if (path.StartsWith("http://") || path.StartsWith("https://"))
            return await Download(path);

        if (!FileAccess.FileExists(path))
            return null;

        var img = new Image();

        return img.Load(path) == Error.Ok
            ? ImageTexture.CreateFromImage(img)
            : null;
    }

    private async Task<Texture2D?> Download(string url)
    {
        var http = new HttpRequest();
        AddChild(http);

        if (http.Request(url) != Error.Ok)
        {
            http.QueueFree();
            return null;
        }

        var result = await ToSignal(
            http,
            HttpRequest.SignalName.RequestCompleted);

        http.QueueFree();

        var body = result[3].AsByteArray();
        var img = new Image();

        if (img.LoadPngFromBuffer(body) != Error.Ok &&
            img.LoadJpgFromBuffer(body) != Error.Ok &&
            img.LoadWebpFromBuffer(body) != Error.Ok)
        {
            return null;
        }

        return ImageTexture.CreateFromImage(img);
    }

    private void Refresh()
    {
        _count.Text = _up.ToString();
        EmitSignal(SignalName.ScoreChanged);
    }
}
