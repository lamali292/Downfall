using Downfall.DownfallCode.Voting;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Voting;

public partial class NArtVotingRow : NClickableControl
{
    private Panel _selectionHighlight = null!;
    private bool _isSelected;
    private const string ScenePath = "res://Downfall/scenes/voting/art_voting_row.tscn";
    private const float SelectedAlpha = 0.25f;
    private NArtVotingScreen _screen = null!;

    public ArtData? ArtData { get; private set; }

    public void SetSelected(bool isSelected)
    {
        if (_isSelected == isSelected)
            return;

        _isSelected = isSelected;

        if (_isSelected)
            _selectionHighlight.Modulate = StsColors.blue with { A = SelectedAlpha };
        else if (IsFocused)
            _selectionHighlight.Modulate = StsColors.darkBlue with { A = SelectedAlpha };
        else
            _selectionHighlight.Modulate = Colors.Transparent;
    }

    public static NArtVotingRow? Create(NArtVotingScreen screen, ArtData artData)
    {
        if (TestMode.IsOn)
            return null;

        var row = PreloadManager.Cache.GetScene(ScenePath).Instantiate<NArtVotingRow>();
        row.ArtData = artData;
        row._screen = screen;
        return row;
    }
    
    public override void _Ready()
    {
        if (ArtData == null)
            return;

        _selectionHighlight = GetNode<Panel>("SelectionHighlight");
        var title = GetNode<MegaRichTextLabel>("Title");
        var platformIcon = GetNode<TextureRect>("Icon");

        _selectionHighlight.Modulate = _selectionHighlight.Modulate with { A = 0f };

        var card = ArtData.Card;
        if (card ==  null) return;
        title.Text = card.Title;
        var texture = GetIcon(card.Pool);
        if (texture != null) platformIcon.Texture = texture;
        ConnectSignals();
    }

    protected override void OnFocus()
    {
        if (_isSelected)
            return;
        _selectionHighlight.Modulate = StsColors.darkBlue with { A = SelectedAlpha };
    }

    protected override void OnUnfocus()
    {
        if (_isSelected)
            return;

        _selectionHighlight.Modulate = Colors.Transparent;
    }

    protected override void OnRelease() => _screen.OnRowSelected(this);
    
    private static Texture2D? GetIcon(CardPoolModel poolModel)
    {
        return ModelDb.AllCharacters.FirstOrDefault(e => e.CardPool == poolModel)?.IconTexture;
    }
}