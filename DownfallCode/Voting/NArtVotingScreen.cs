using Downfall.DownfallCode.Voting;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Screens.MainMenu;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Voting;

public partial class NArtVotingScreen : NSubmenu
{
    private const string ScenePath = "res://Downfall/scenes/voting/voting.tscn";
    private NArtVotingContainer _submissionsContainer = null!;
    private NArtVotingCardContainer _cardContainer = null!;
    private Control _modRowContainer = null!;

    public static NArtVotingScreen? Create()
    {
        return TestMode.IsOn ? null : PreloadManager.Cache.GetScene(ScenePath).Instantiate<NArtVotingScreen>();
    }

    public void OnRowSelected(NArtVotingRow row)
    {
        row.SetSelected(true);
        if (row.ArtData != null)
        {
            _cardContainer.Fill(row.ArtData);
            _submissionsContainer.Fill(row.ArtData);
        }

        foreach (var nmodMenuRow in _modRowContainer.GetChildren().OfType<NArtVotingRow>())
        {
            if (nmodMenuRow != row)
                nmodMenuRow.SetSelected(false);
        }
    }

    public void AddArtData(ArtData artData)
    {
        _modRowContainer.AddChildSafely(NArtVotingRow.Create(this, artData));
    }

    private async Task LoadCategories()
    {
        var categories = await VotingApi.Instance.GetCategories();
        if (categories == null)
        {
            GD.PrintErr("Failed to load categories");
            return;
        }

        foreach (var category in categories)
            AddArtData(category);

        var first = _modRowContainer.GetChildren().OfType<NArtVotingRow>().FirstOrDefault();
        if (first != null)
            OnRowSelected(first);
    }

    private static LocString RowTitle => new("settings_ui", "DOWNFALL-VOTING_SCREEN.ROW_TITLE");

    public override void _Ready()
    {
        _submissionsContainer = GetNode<NArtVotingContainer>("%SubmissionsContainer");
        _modRowContainer = GetNode<Control>("%ModsScrollContainer/Mask/Content");
        _cardContainer = GetNode<NArtVotingCardContainer>("%CardContainer");

        foreach (var child in _modRowContainer.GetChildren())
            child.QueueFreeSafely();

        GetNode<MegaRichTextLabel>((NodePath)"%ArtVotingTitle")
            .SetTextAutoSize(RowTitle.GetFormattedText());

        // clicking a submission card swaps the card-preview portrait
        _submissionsContainer.EntryClicked += imagePath => _cardContainer.UpdateImage(imagePath);

        ConnectSignals();
        _ = LoadCategories();
    }

    protected override Control? InitialFocusedControl => null;
}