using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// A one-shot sibling of the vanilla <c>NCardViewSortButton</c>
/// (scenes/screens/card_library/library_sort_button.tscn) for
/// <see cref="NVotingFilter"/>'s Top/New/Hot switch. That base button flips
/// its own asc/desc state on every click, which fights a "pick exactly one"
/// switch instead of helping it - so this reuses the same look (same
/// background/icon art, same MegaLabel/SelectionReticle setup) but exposes a
/// plain externally-driven <see cref="IsActive"/> highlight instead.
/// </summary>
public partial class NVotingSortButton : NButton
{
    private static readonly Color ActiveColor = new(0.937f, 0.784f, 0.317f);
    private static readonly Color InactiveColor = new(0.55f, 0.55f, 0.55f);

    private Control _visuals = null!;
    private TextureRect _buttonImage = null!;
    private MegaLabel _label = null!;
    private TextureRect _icon = null!;
    private NSelectionReticle _selectionReticle = null!;
    private Tween? _tween;

    private bool _isActive;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            Refresh();
        }
    }

    public override void _Ready()
    {
        ConnectSignals();
        _visuals = GetNode<Control>("%Visuals");
        _buttonImage = GetNode<TextureRect>("%ButtonImage");
        _label = GetNode<MegaLabel>("%Label");
        _icon = GetNode<TextureRect>("%Image");
        _selectionReticle = GetNode<NSelectionReticle>("SelectionReticle");

        // Scale is about the top-left corner by default; the button's width
        // isn't known until the VBoxContainer parent lays it out, so the
        // center pivot has to be kept in sync as it resizes instead of being
        // set once here.
        _visuals.Resized += () => _visuals.PivotOffset = _visuals.Size * 0.5f;
        _visuals.PivotOffset = _visuals.Size * 0.5f;

        Refresh();
    }

    public void SetLabel(string text) => _label.SetTextAutoSize(text);

    private void Refresh()
    {
        var color = _isActive ? ActiveColor : InactiveColor;
        _label.Modulate = color;
        _icon.Modulate = color;
        _buttonImage.Modulate = _isActive ? Colors.White : new Color(0.6f, 0.6f, 0.6f);
        _visuals.Scale = _isActive ? new Vector2(1.05f, 1.05f) : Vector2.One;
    }

    protected override void OnFocus()
    {
        base.OnFocus();
        Bump(1.05f);
        if (NControllerManager.Instance?.IsUsingDirectionalNavigation == true)
            _selectionReticle.OnSelect();
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        Bump(_isActive ? 1.05f : 1f);
        if (NControllerManager.Instance?.IsUsingDirectionalNavigation == true)
            _selectionReticle.OnDeselect();
    }

    protected override void OnPress()
    {
        base.OnPress();
        Bump(0.95f);
    }

    private void Bump(float scale)
    {
        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(_visuals, "scale", new Vector2(scale, scale), 0.15)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
    }
}
