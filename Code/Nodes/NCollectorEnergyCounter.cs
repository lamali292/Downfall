using Downfall.Code.Core.Collector;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.addons.mega_text;

namespace Downfall.Code.Nodes;

[GlobalClass]
public partial class NCollectorEnergyCounter : Control
{
    private Player?      _player;
    private MegaLabel?   _label;
    private TextureRect? _icon;

    private const int LabelX    = 100;
    private const int LabelY    = 90;
    private const int LabelSize = 100;

    public void Initialize(Player player)
    {
        _player = player;
        CollectorEnergy.Changed += OnEnergyChanged;
    }

    public override void _Ready()
    {
        _icon = new TextureRect
        {
            Texture     = ResourceLoader.Load<Texture2D>("res://Downfall/images/ui/collector_energy.png"),
            StretchMode = TextureRect.StretchModeEnum.KeepAspect,
            Size        = new Vector2(LabelSize, LabelSize),
            Position    = new Vector2(LabelX, LabelY)
        };
        AddChild(_icon);

        _label = new MegaLabel
        {
            Size                = new Vector2(LabelSize, LabelSize),
            Position            = new Vector2(LabelX, LabelY),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment   = VerticalAlignment.Center,
            MaxFontSize         = 38,
            MinFontSize         = 25,
        };

        // MegaLabel requires a font override or it throws in _Ready
        var font = ResourceLoader.Load<Font>("res://addons/mega_text/fonts/regular.ttf");
        _label.AddThemeFontOverride(ThemeConstants.Label.Font, font);
        _label.AddThemeConstantOverride("outline_size", 6);
        _label.AddThemeColorOverride(ThemeConstants.Label.FontColor, StsColors.cream);
        _label.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor, new Color("900000"));

        AddChild(_label);

        Size    = new Vector2(LabelSize, LabelSize);
        Visible = false;
        Refresh();
    }

    public override void _ExitTree()
    {
        CollectorEnergy.Changed -= OnEnergyChanged;
    }

    private void OnEnergyChanged(Player player, int amount)
    {
        if (player != _player) return;
        Refresh();
    }

    private Tween? _fadeTween;

    private void Refresh()
    {
        if (_player == null || _label == null) return;
        var amount = CollectorEnergy.Get(_player);

        _label.SetTextAutoSize(amount.ToString());
        _label.AddThemeColorOverride(ThemeConstants.Label.FontColor,
            amount == 0 ? StsColors.red : new Color("EBFFAD"));
        _label.AddThemeColorOverride(ThemeConstants.Label.FontOutlineColor,
            amount == 0 ? StsColors.unplayableEnergyCostOutline : new Color("3A3F2B"));

        var targetAlpha = amount > 0 ? 1f : 0f;

        if (!Visible && amount > 0)
        {
            Visible      = true;
            Modulate     = new Color(1, 1, 1, 0f);
        }
        _fadeTween?.Kill();
        _fadeTween = CreateTween();
        _fadeTween.TweenProperty(this, "modulate:a", targetAlpha, 0.3)
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);

        if (amount == 0)
            _fadeTween.TweenCallback(Callable.From(() => Visible = false));
    }
}