using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.TopBar;

namespace Downfall.DownfallCode.Utils.UI;

/// <summary>
///     Abstract base for top bar elements that are full buttons (press/hover/screen
///     animations via <see cref="NTopBarButton" />). Handles count badge and rocking
///     animation; subclasses supply scene path, player filter, count source, and
///     click behaviour.
/// </summary>
public abstract partial class NCustomTopBarButton : NTopBarButton, ITopBarElement
{
    private const float RockSpeed = 4f;
    private const float RockDist = 0.12f;

    private static NCustomTopBarButton? _instance;
    private Tween? _bumpTween;

    private MegaLabel? _countLabel;

    private float _elapsedTime;
    private float _previousCount;
    private float _rockBaseRotation;
    protected Player? Player;
    public static Vector2 ButtonPosition => _instance?.GlobalPosition ?? Vector2.Zero;
    public static Vector2 ButtonSize => _instance?.Size ?? Vector2.Zero;


    public abstract string ScenePath { get; }
    public abstract float Width { get; }
    public abstract Func<Player, bool> CanUse { get; }

    public void Initialize(Player player)
    {
        Player = player;
        _instance = this;
        RefreshCount();
    }


    public override void _Ready()
    {
        ConnectSignals();
        _icon = GetNode<Control>("Control/Icon");
        _hsv = (ShaderMaterial)_icon.Material;
        _countLabel = GetNodeOrNull<MegaLabel>("DeckCardCount");
    }


    /// <summary>Returns the value to show on the badge, or null to hide it.</summary>
    protected abstract int? GetCount();

    private void RefreshCount()
    {
        if (_countLabel == null) return;
        var count = GetCount();

        if (count == null)
        {
            _countLabel.Visible = false;
            return;
        }

        _countLabel.Visible = true;

        if (count > _previousCount)
        {
            _bumpTween?.Kill();
            _bumpTween = CreateTween();
            _bumpTween.TweenProperty(_countLabel, "scale", Vector2.One, 0.5f)
                .From(Vector2.One * 1.5f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Expo);
            _countLabel.PivotOffset = _countLabel.Size * 0.5f;
        }

        _previousCount = count.Value;
        _countLabel.SetTextAutoSize(count.Value.ToString());
    }

    public override void _Process(double delta)
    {
        if (!IsScreenOpen) return;
        _elapsedTime += (float)delta * RockSpeed;
        _icon.Rotation = _rockBaseRotation + RockDist * Mathf.Sin(_elapsedTime);
        _rockBaseRotation = (float)Mathf.Lerp(_rockBaseRotation, 0.0, delta);
    }

    public static void RefreshButton()
    {
        _instance?.RefreshCount();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_instance == this) _instance = null;
    }
}