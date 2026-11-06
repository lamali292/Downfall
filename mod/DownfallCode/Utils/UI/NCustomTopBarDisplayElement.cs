using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Downfall.DownfallCode.Utils.UI;

public abstract partial class NCustomTopBarDisplayElement : NClickableControl, ITopBarElement
{
    private Tween? _bumpTween;
    private MegaLabel? _countLabel;
    private float _elapsedTime;
    private Control? _icon;
    private float _previousCount;
    protected Player? Player;

    protected abstract string IconNodePath { get; }
    protected abstract string CountLabelNodePath { get; }

    public virtual void Initialize(Player player)
    {
        Player = player;
        RefreshCount();
    }

    public override void _Ready()
    {
        ConnectSignals();
        _icon = GetNode<Control>(IconNodePath);
        _countLabel = GetNodeOrNull<MegaLabel>(CountLabelNodePath);
    }

    protected abstract int? GetCount();

    public void RefreshCount()
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
}