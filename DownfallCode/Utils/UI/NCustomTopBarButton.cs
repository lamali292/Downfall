using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.TopBar;

namespace Downfall.DownfallCode.Utils.UI;

public abstract partial class NCustomTopBarButton : NTopBarButton, ITopBarElement
{
    private const float RockSpeed = 4f;
    private const float RockDist = 0.12f;

    private Tween? _bumpTween;
    private MegaLabel? _countLabel;
    private float _elapsedTime;
    private float _previousCount;
    private float _rockBaseRotation;
    protected Player? Player;

    public virtual void Initialize(Player player)
    {
        Player = player;
        RefreshCount();
    }

    public override void _Ready()
    {
        ConnectSignals();
        _icon = GetNode<Control>("Control/Icon");
        _hsv = (ShaderMaterial)_icon.Material;
        _countLabel = GetNodeOrNull<MegaLabel>("DeckCardCount");
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
        if (!IsScreenOpen) return;
        _elapsedTime += (float)delta * RockSpeed;
        _icon.Rotation = _rockBaseRotation + RockDist * Mathf.Sin(_elapsedTime);
        _rockBaseRotation = (float)Mathf.Lerp(_rockBaseRotation, 0.0, delta);
    }
}