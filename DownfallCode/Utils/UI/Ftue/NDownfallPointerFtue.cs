using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Ftue;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Downfall.DownfallCode.Utils.UI.Ftue;

public partial class NDownfallPointerFtue : NFtue
{
    private const float ArrowHeading = 2.356f; // the arrow art points down-left, in screen radians
    private static readonly Vector2 ArrowSize = new(244, 204);
    // Pixel location of the arrowhead's point within the 244x204 art (measured from the source PNG) —
    // must match the Arrow node's pivot_offset in pointer_ftue.tscn, since that's what it rotates around.
    private static readonly Vector2 ArrowTip = new(28, 195);
    private static readonly Vector2 PopupSize = new(579, 257);
    private const float ScreenMargin = 20f;

    private TextureRect _popup = null!;
    private MegaLabel _header = null!;
    private MegaRichTextLabel _description = null!;
    private NButton _confirm = null!;
    private TextureRect _arrow = null!;

    // Set by DownfallFtue when this popup is one of several shown together on the same screen:
    // dismissing it then only removes itself instead of clearing the whole modal (see DownfallFtue.ShowCombo).
    public Action? OnDismissed;

    // Re-queried whenever the viewport resizes, so the arrow keeps following its target instead of
    // staying pinned to where the target used to be before the rescale.
    private Func<DownfallFtue.PointerTarget?>? _findTarget;
    private Vector2 _arrowFromTarget;
    private Vector2 _popupFromArrow;
    private float? _pointDirectionDegrees;

    public override void _Ready()
    {
        _popup = GetNode<TextureRect>("%FtuePopup");
        _header = GetNode<MegaLabel>("%Header");
        _description = GetNode<MegaRichTextLabel>("%Description");
        _confirm = GetNode<NButton>("%FtueConfirmButton");
        _arrow = GetNode<TextureRect>("%Arrow");
        _confirm.Connect(NClickableControl.SignalName.Released, Callable.From((NButton _) => Dismiss()));
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_findTarget != null) GetViewport().SizeChanged -= RetrackTarget;
    }

    private void Dismiss()
    {
        if (OnDismissed is { } onDismissed)
        {
            onDismissed();
            this.QueueFreeSafely();
        }
        else
        {
            CloseFtue();
        }
    }

    public void SetText(string title, string body)
    {
        _header.SetTextAutoSize(title);
        _description.Text = body;
    }
    
    // `arrowFromTarget` places the arrow's TIP (not its center) — rotation pivots around that tip, so
    // swinging the arrow to point in a different direction keeps the tip anchored and swings the tail.
    // `pointDirectionDegrees`, when given, is the exact screen direction (0 = right, 90 = down, Godot
    // convention) the arrow should visually point, overriding the default "aim straight at the target".
    public void PointAt(Vector2 aim, Vector2 arrowFromTarget, Vector2 popupFromArrow, float? pointDirectionDegrees = null)
    {
        var origin = GlobalPosition;
        var view = GetViewportRect().Size;
        var tipAnchor = aim + arrowFromTarget;
        _arrow.Position = tipAnchor - ArrowTip - origin;
        var pointDirection = pointDirectionDegrees is { } degrees
            ? Mathf.DegToRad(degrees)
            : (aim - tipAnchor).Angle();
        _arrow.Rotation = pointDirection - ArrowHeading;
        var popupPos = tipAnchor + popupFromArrow + new Vector2(0f, -ArrowSize.Y / 2f - PopupSize.Y);
        _popup.Position = popupPos.Clamp(new Vector2(ScreenMargin, ScreenMargin),
            view - PopupSize - new Vector2(ScreenMargin, ScreenMargin)) - origin;
    }

    // Points at the target now, and keeps re-pointing at it (re-querying `findTarget`, not just
    // reusing the first result) whenever the viewport resizes, so a rescale while the tip is up
    // doesn't leave the arrow pointing at empty space.
    public void Track(Func<DownfallFtue.PointerTarget?> findTarget, Vector2 arrowFromTarget, Vector2 popupFromArrow,
        float? pointDirectionDegrees = null)
    {
        _findTarget = findTarget;
        _arrowFromTarget = arrowFromTarget;
        _popupFromArrow = popupFromArrow;
        _pointDirectionDegrees = pointDirectionDegrees;
        GetViewport().SizeChanged += RetrackTarget;
        RetrackTarget();
    }

    private void RetrackTarget()
    {
        if (_findTarget?.Invoke() is { } target)
            PointAt(target.Aim, _arrowFromTarget, _popupFromArrow, _pointDirectionDegrees);
    }
}
