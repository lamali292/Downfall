using Downfall.Code.Extensions;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;

namespace Downfall.Code.Nodes;

[GlobalClass]
public partial class NTopBarEssenceDisplay : NClickableControl
{
    private static NTopBarEssenceDisplay? _instance;
    
    private Player? _player;
    private MegaLabel? _countLabel;
    private Control? _icon;
    private float _elapsedTime;
    private Tween? _bumpTween;
    private float _count;

    public static void RefreshDisplay() => _instance?.RefreshCount();

    public override void _Ready()
    {
        ConnectSignals();
        _icon = GetNode<Control>("Control");
        _countLabel = GetNode<MegaLabel>("EssenceCount");
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (_instance == this) _instance = null;
    }

    public void Initialize(Player player)
    {
        _instance = this;
        _player = player;
        RefreshCount();
    }

    public override void _Process(double delta)
    {
        if (!IsFocused) return;
        _elapsedTime += (float)delta * 4f;
        _icon.Rotation = 0.12f * Mathf.Sin(_elapsedTime);
    }

    protected override void OnFocus()
    {
        base.OnFocus();
        _elapsedTime = 0;
    }

    protected override void OnUnfocus()
    {
        base.OnUnfocus();
        _icon.Rotation = 0f;
    }

    private void RefreshCount()
    {
        if (_countLabel == null || _player == null) return;
        var count = _player.GetEssence();
        if (count > _count)
        {
            _bumpTween?.Kill();
            _bumpTween = CreateTween();
            _bumpTween.TweenProperty(_countLabel, "scale", Vector2.One, 0.5f)
                .From(Vector2.One * 1.5f)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Expo);
            _countLabel.PivotOffset = _countLabel.Size * 0.5f;
        }
        _count = count;
        _countLabel.SetTextAutoSize(count.ToString());
    }
}