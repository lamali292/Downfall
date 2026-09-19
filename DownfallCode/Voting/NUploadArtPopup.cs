using Godot;
using MegaCrit.Sts2.Core.Helpers;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Card-art upload flow. Fully self-contained: it computes its own list of
/// cards missing art via <see cref="MissingArtCards"/> rather than requiring
/// a caller to hand one in, so any screen can open it with
/// <see cref="OpenFrom"/> without needing to know anything about the voting
/// grid or its state.
/// </summary>
public partial class NUploadArtPopup : Control
{
    public const string ScenePath = "res://Downfall/scenes/voting/upload_popup.tscn";

    private static NUploadArtPopup? _current;

    /// <param name="onUploaded">
    /// Fired after a successful upload, with the card it was submitted for -
    /// e.g. so a visible voting grid can refresh that one card's submissions.
    /// Purely optional; the popup itself doesn't need it.
    /// </param>
    public static void OpenFrom(Node parent, Action<ArtData>? onUploaded = null)
    {
        if (GodotObject.IsInstanceValid(_current))
            return;

        var popup = GD.Load<PackedScene>(ScenePath).Instantiate<NUploadArtPopup>();
        _current = popup;
        popup.TreeExiting += () => _current = null;
        parent.AddChild(popup);
        popup.Open(onUploaded);
    }

    private ColorRect _dim = null!;
    private Label _title = null!;
    private Label _categoryLabel = null!;
    private Button _chooseCardButton = null!;
    private TextureRect _preview = null!;
    private Button _chooseButton = null!;
    private Label _status = null!;
    private Button _cancelButton = null!;
    private Button _submitButton = null!;
    private Label _rulesTitle = null!;
    private RichTextLabel _rulesText = null!;
    private Label _guidelinesTitle = null!;
    private RichTextLabel _guidelinesText = null!;
    private Label _sizeHint = null!;
    private Label _creditNameLabel = null!;
    private LineEdit _creditNameEdit = null!;

    private ArtData? _selectedCategory;
    private string? _selectedPath;
    private bool _sizeValid;
    private Action<ArtData>? _onUploaded;

    public override void _Ready()
    {
        _dim = GetNode<ColorRect>("%Dim");
        _title = GetNode<Label>("%Title");
        _categoryLabel = GetNode<Label>("%CategoryLabel");
        _chooseCardButton = GetNode<Button>("%ChooseCardButton");
        _preview = GetNode<TextureRect>("%Preview");
        _chooseButton = GetNode<Button>("%ChooseButton");
        _status = GetNode<Label>("%Status");
        _cancelButton = GetNode<Button>("%CancelButton");
        _submitButton = GetNode<Button>("%SubmitButton");
        _rulesTitle = GetNode<Label>("%RulesTitle");
        _rulesText = GetNode<RichTextLabel>("%RulesText");
        _guidelinesTitle = GetNode<Label>("%GuidelinesTitle");
        _guidelinesText = GetNode<RichTextLabel>("%GuidelinesText");
        _sizeHint = GetNode<Label>("%SizeHint");
        _creditNameLabel = GetNode<Label>("%CreditNameLabel");
        _creditNameEdit = GetNode<LineEdit>("%CreditNameEdit");

        _title.Text = VotingUi.Loc("DOWNFALL-VOTING.upload_title");
        _categoryLabel.Text = VotingUi.Loc("DOWNFALL-VOTING.category_label");
        _rulesTitle.Text = VotingUi.Loc("DOWNFALL-VOTING.rules_title");
        _rulesText.Text = VotingUi.Loc("DOWNFALL-VOTING.rules_text");
        _guidelinesTitle.Text = VotingUi.Loc("DOWNFALL-VOTING.guidelines_title");
        _guidelinesText.Text = VotingUi.Loc("DOWNFALL-VOTING.guidelines_text");
        _creditNameLabel.Text = VotingUi.Loc("DOWNFALL-VOTING.credit_name_label");
        _creditNameEdit.TooltipText = VotingUi.Loc("DOWNFALL-VOTING.credit_name_hint");

        VotingUi.StyleActionButton(_chooseCardButton, primary: false);
        VotingUi.StyleActionButton(_chooseButton, primary: true);
        VotingUi.StyleActionButton(_cancelButton, primary: false);
        VotingUi.StyleActionButton(_submitButton, primary: true);
        _chooseCardButton.Text = VotingUi.Loc("DOWNFALL-VOTING.choose_card_button");
        _chooseButton.Text = VotingUi.Loc("DOWNFALL-VOTING.choose_image_button");
        _cancelButton.Text = VotingUi.Loc("DOWNFALL-VOTING.cancel_button");
        _submitButton.Text = VotingUi.Loc("DOWNFALL-VOTING.submit_button");

        _dim.GuiInput += e =>
        {
            if (e is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
                QueueFree();
        };
        _cancelButton.Pressed += QueueFree;
        _chooseCardButton.Pressed += OnChooseCardPressed;
        _chooseButton.Pressed += () => VotingUi.PickImageFile(this, OnImageChosen);
        _submitButton.Pressed += () => TaskHelper.RunSafely(Submit());
    }

    private void Open(Action<ArtData>? onUploaded)
    {
        _onUploaded = onUploaded;
        RevalidateSelectedImage();

        _creditNameEdit.Text = Steamworks.SteamFriends.GetPersonaName() ?? "";

        // Only fetches if a session already exists - Submit() signs in
        // lazily, so a first-time uploader won't have one yet at Open() time,
        // and the Steam persona name above is already a reasonable default
        // for that case.
        if (VotingAuth.IsSignedIn)
            TaskHelper.RunSafely(LoadSavedCreditName());
    }

    private async Task LoadSavedCreditName()
    {
        var saved = await VotingApi.Instance.GetMyCreditName();
        if (IsInstanceValid(this) && !string.IsNullOrEmpty(saved))
            _creditNameEdit.Text = saved;
    }

    private void OnChooseCardPressed()
    {
        NSelectCardPopup.OpenFrom(this, cat =>
        {
            _selectedCategory = cat;
            _chooseCardButton.Text = cat.Card?.Title ?? cat.ModelId.Entry;
            RevalidateSelectedImage();
        });
    }

    private void OnImageChosen(string path)
    {
        _selectedPath = path;
        var img = new Image();
        if (img.Load(path) == Error.Ok)
            _preview.Texture = ImageTexture.CreateFromImage(img);
        RevalidateSelectedImage();
    }

    private void RevalidateSelectedImage()
    {
        _sizeHint.Text = MissingArtCards.SizeRequirementText(_selectedCategory);

        if (_selectedPath == null)
            return;

        var img = new Image();
        if (img.Load(_selectedPath) != Error.Ok)
            return;

        _sizeValid = MissingArtCards.IsValidSize(img.GetWidth(), img.GetHeight(), _selectedCategory);
        _status.Text = _sizeValid
            ? ""
            : VotingUi.Loc("DOWNFALL-VOTING.error_size_mismatch",
                ("width", img.GetWidth().ToString()),
                ("height", img.GetHeight().ToString()),
                ("requirement", MissingArtCards.SizeRequirementText(_selectedCategory)));
    }

    private async Task Submit()
    {
        if (_selectedPath == null)
        {
            _status.Text = VotingUi.Loc("DOWNFALL-VOTING.error_choose_image");
            return;
        }

        if (_selectedCategory is not { } category)
        {
            _status.Text = VotingUi.Loc("DOWNFALL-VOTING.error_no_category");
            return;
        }

        if (!_sizeValid)
        {
            _status.Text = VotingUi.Loc("DOWNFALL-VOTING.error_wrong_size",
                ("requirement", MissingArtCards.SizeRequirementText(category)));
            return;
        }

        _submitButton.Disabled = true;

        if (!VotingAuth.IsSignedIn)
        {
            _status.Text = VotingUi.Loc("DOWNFALL-VOTING.status_signing_in");
            var (ok, message) = await VotingAuth.LoginAsync();
            if (!ok)
            {
                _status.Text = message;
                _submitButton.Disabled = false;
                return;
            }
        }

        _status.Text = VotingUi.Loc("DOWNFALL-VOTING.status_uploading");

        var creditName = _creditNameEdit.Text.Trim();
        var (renamed, renameError) = await VotingApi.Instance.SetMyCreditName(string.IsNullOrEmpty(creditName) ? "Anonymous" : creditName);
        if (!renamed && renameError != null)
        {
            // Rename didn't take (e.g. rate-limited) - upload still proceeds
            // under whichever credit name the account already had saved.
            DownfallMainFile.Logger.Info($"[VotingApi] credit-name update skipped: {renameError}");
        }

        var (uploaded, error) = await VotingApi.Instance.UploadSubmission(category.ModelId, _selectedPath);

        if (!uploaded)
        {
            _status.Text = error;
            _submitButton.Disabled = false;
            return;
        }

        _status.Text = VotingUi.Loc("DOWNFALL-VOTING.status_uploaded");
        _onUploaded?.Invoke(category);
        QueueFree();
    }
}
