using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Downfall.DownfallCode.Config.UI;

/// <summary>
/// A small icon-only "reset" button showing a loop-arrow glyph as text.
/// Brightens and grows slightly on hover/focus like the other STS2 icon buttons.
/// </summary>
public partial class NResetIconButton : NClickableControl
{
    private static readonly Color Idle = new(0.91f, 0.86f, 0.74f, 0.75f);
    private static readonly Color Hovered = new(1f, 0.96f, 0.82f);

    private readonly Label _label;
    private Tween? _tween;

    public NResetIconButton(Action onPressed)
    {
        CustomMinimumSize = new Vector2(44, 44);
        PivotOffset = CustomMinimumSize / 2;
        SizeFlagsHorizontal = SizeFlags.ShrinkCenter;
        SizeFlagsVertical = SizeFlags.ShrinkCenter;
        FocusMode = FocusModeEnum.All;
        MouseFilter = MouseFilterEnum.Stop;
        Connect(NClickableControl.SignalName.Released, Callable.From<NClickableControl>(_ => onPressed()));

        _label = new Label
        {
            Text = "↻",
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            MouseFilter = MouseFilterEnum.Ignore
        };
        _label.SetAnchorsPreset(LayoutPreset.FullRect);
        _label.AddThemeFontOverride("font", PreloadManager.Cache.GetAsset<FontVariation>("res://themes/kreon_bold_glyph_space_two.tres"));
        _label.AddThemeFontSizeOverride("font_size", 36);
        _label.AddThemeColorOverride("font_color", Idle);
        AddChild(_label);
    }

    public override void _Ready() => ConnectSignals();

    protected override void OnFocus() => Animate(focused: true);

    protected override void OnUnfocus() => Animate(focused: false);

    private void Animate(bool focused)
    {
        _label.AddThemeColorOverride("font_color", focused ? Hovered : Idle);
        _tween?.Kill();
        _tween = CreateTween();
        _tween.TweenProperty(this, "scale", Vector2.One * (focused ? 1.15f : 1f), 0.1);
    }
}
