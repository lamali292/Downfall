using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.addons.mega_text;

namespace Downfall.Code.Utils.UI;

/// <summary>
/// Abstract base for top bar elements that are display widgets — no press effect,
/// just a hover wobble and an optional count badge. Subclasses supply scene path,
/// player filter, icon node name, count label node name, and count source.
/// </summary>
public abstract partial class NCustomTopBarDisplayElement : NClickableControl, ITopBarElement
{
    protected Player? Player;

    private Control? _icon;
    private MegaLabel? _countLabel;
    private Tween? _bumpTween;
    private float _previousCount;
    private float _elapsedTime;

    // ── ITopBarElement ────────────────────────────────────────────────────────

    public abstract string ScenePath { get; }
    public abstract float Width { get; }
    public abstract Func<Player, bool> CanUse { get; }

    public void Initialize(Player player)
    {
        Player = player;
        _instance = this;
        RefreshCount();
    }
    

    // ── Lifecycle ─────────────────────────────────────────────────────────────

    /// <summary>Node path to the icon Control that wobbles on hover.</summary>
    protected abstract string IconNodePath { get; }

    /// <summary>Node path to the MegaLabel showing the count badge.</summary>
    protected abstract string CountLabelNodePath { get; }

    public override void _Ready()
    {
        ConnectSignals();
        _icon = GetNode<Control>(IconNodePath);
        _countLabel = GetNodeOrNull<MegaLabel>(CountLabelNodePath);
    }

    // ── Count badge ───────────────────────────────────────────────────────────

    /// <summary>Returns the value to show on the badge, or null to hide it.</summary>
    protected abstract int? GetCount();

    public void RefreshCount()
    {
        if (_countLabel == null) return;
        var count = GetCount();

        if (count == null) { _countLabel.Visible = false; return; }
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

    // ── Hover wobble ──────────────────────────────────────────────────────────

    public override void _Process(double delta)
    {
        if (!IsFocused) return;
        _elapsedTime += (float)delta * 4f;
        _icon!.Rotation = 0.12f * Mathf.Sin(_elapsedTime);
    }

    protected override void OnFocus()
    {
        base.OnFocus();
        _elapsedTime = 0;
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        _icon!.Rotation = 0f;
    }
    
    private static NCustomTopBarDisplayElement? _instance;
    public static void RefreshDisplay() => _instance?.RefreshCount();
    
    public override void _ExitTree()
    {
        base._ExitTree();
        if (_instance == this) _instance = null;
    }
}