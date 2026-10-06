using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Downfall.DownfallCode.Utils.UI.Ftue;

public partial class NDownfallPointerFtue : NFtue
{
    private const float ArrowHeading = 2.356f; // the arrow art points down-left, in screen radians
    private static readonly Vector2 ArrowSize = new(244, 204);
    private static readonly Vector2 PopupSize = new(579, 257);
    private const float ScreenMargin = 20f;

    private TextureRect _popup = null!;
    private MegaLabel _header = null!;
    private MegaRichTextLabel _description = null!;
    private NButton _confirm = null!;
    private TextureRect _arrow = null!;

    public override void _Ready()
    {
        _popup = GetNode<TextureRect>("%FtuePopup");
        _header = GetNode<MegaLabel>("%Header");
        _description = GetNode<MegaRichTextLabel>("%Description");
        _confirm = GetNode<NButton>("%FtueConfirmButton");
        _arrow = GetNode<TextureRect>("%Arrow");
        _confirm.Connect(NClickableControl.SignalName.Released, Callable.From((NButton _) => CloseFtue()));
    }

    public void SetText(string title, string body)
    {
        _header.SetTextAutoSize(title);
        _description.Text = body;
    }
    
    public void PointAt(Vector2 aim, Vector2 arrowFromTarget, Vector2 popupFromArrow)
    {
        var origin = GlobalPosition;
        var view = GetViewportRect().Size;
        var arrowCenter = aim + arrowFromTarget;
        _arrow.Position = arrowCenter - ArrowSize / 2f - origin;
        _arrow.Rotation = (aim - arrowCenter).Angle() - ArrowHeading;
        var popupPos = arrowCenter + popupFromArrow + new Vector2(0f, -ArrowSize.Y / 2f - PopupSize.Y);
        _popup.Position = popupPos.Clamp(new Vector2(ScreenMargin, ScreenMargin),
            view - PopupSize - new Vector2(ScreenMargin, ScreenMargin)) - origin;
    }
}
