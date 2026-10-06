using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Downfall.DownfallCode.Utils.UI.Ftue;

public abstract partial class NDownfallRulesFtue : NFtue
{
    private Texture2D? Image1 => ResourceLoader.Load<Texture2D>(ImagePath1);
    private Texture2D? Image2 => ResourceLoader.Load<Texture2D>(ImagePath2);
    private Texture2D? Image3 => ResourceLoader.Load<Texture2D>(ImagePath3);

    protected abstract string? ImagePath1 { get; }
    protected abstract string? ImagePath2 { get; }
    protected abstract string? ImagePath3 { get; }
    
    private TextureRect _image = null!;
    private MegaLabel _header = null!;
    private MegaLabel _pageCount = null!;
    private MegaRichTextLabel _description = null!;
    private NGoldArrowButton _prevButton = null!;
    private NGoldArrowButton _nextButton = null!;

    private Texture2D?[] _images = [];
    private string[] _pages = [];
    private int _currentPage; // 0-based

    public override void _Ready()
    {
        _image = GetNode<TextureRect>("%Image");
        _header = GetNode<MegaLabel>("%Header");
        _pageCount = GetNode<MegaLabel>("%PageCount");
        _description = GetNode<MegaRichTextLabel>("%Description");
        _prevButton = GetNode<NGoldArrowButton>("%LeftArrow");
        _nextButton = GetNode<NGoldArrowButton>("%RightArrow");
        _prevButton.Connect(NClickableControl.SignalName.Released, Callable.From((NButton _) => Page(-1)));
        _nextButton.Connect(NClickableControl.SignalName.Released, Callable.From((NButton _) => Page(1)));
        _images = [Image1, Image2, Image3];
    }

    // `pages` is this tip's body text, one entry per page; `title` stays on screen throughout.
    public void SetText(string title, string[] pages)
    {
        _header.SetTextAutoSize(title);
        _pages = pages;
        _currentPage = 0;
        ShowPage();
    }

    // Right arrow doubles as "done" on the last page, same as the base game's own tutorial
    private void Page(int delta)
    {
        _currentPage += delta;
        if (_currentPage >= _pages.Length)
        {
            CloseFtue();
            return;
        }

        ShowPage();
    }

    private void ShowPage()
    {
        _description.SetTextAutoSize(_pages[_currentPage]);
        _image.Texture = _currentPage < _images.Length ? _images[_currentPage] : null;
        _image.Visible = _image.Texture != null;

        _prevButton.Visible = _currentPage > 0;
        if (_currentPage > 0) _prevButton.Enable();
        else _prevButton.Disable();

        var pageCount = new LocString("ftues", "COMBAT_BASICS_FTUE_PAGE_COUNT");
        pageCount.Add("totalPages", (decimal)_pages.Length);
        pageCount.Add("currentPage", _currentPage + 1);
        _pageCount.SetTextAutoSize(pageCount.GetFormattedText());
    }
}
