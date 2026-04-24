using Godot;

namespace Downfall.Code.Vfx.Hexaghost;

[GlobalClass]
public partial class NFire : Node2D
{
    private Node2D? _red;
    private Node2D? _green;
    private Node2D? _blue;
    private Node2D? _yellow;
    private Node2D? _pink;
    private Node2D? _orange;

    private FireColor _currentColor = FireColor.Red;

    public override void _Ready()
    {
        _red = GetNode<Node2D>("%fire_red");
        _green = GetNode<Node2D>("%fire_green");
        _blue = GetNode<Node2D>("%fire_blue");
        _yellow = GetNode<Node2D>("%fire_yellow");
        _pink = GetNode<Node2D>("%fire_pink");
        _orange = GetNode<Node2D>("%fire_orange");
    }

    public void SetColor(FireColor color)
    {
        _currentColor = color;
        if (_red != null) _red.Visible = color == FireColor.Red;
        if (_green != null) _green.Visible = color == FireColor.Green;
        if (_blue != null) _blue.Visible = color == FireColor.Blue;
        if (_yellow != null) _yellow.Visible = color == FireColor.Yellow;
        if (_pink != null) _pink.Visible = color == FireColor.Pink;
        if (_orange != null) _orange.Visible = color == FireColor.Orange;
    }

    private Node2D? GetColorNode(FireColor color) => color switch
    {
        FireColor.Red => _red,
        FireColor.Green => _green,
        FireColor.Blue => _blue,
        FireColor.Yellow => _yellow,
        FireColor.Pink => _pink,
        FireColor.Orange => _orange,
        _ => _red
    };

    public enum FireSize { Large, Small }

    private const float LargeScale = 0.5f;
    private const float SmallScale = 0.25f;

    public FireSize CurrentSize { get; private set; } = FireSize.Small;

    public void SetSize(FireSize size, bool instant = false)
    {
        var target = size == FireSize.Large ? LargeScale : SmallScale;
        CurrentSize = size;

        var colorNode = GetColorNode(_currentColor);
        if (colorNode == null || _green == null) return;
        var isAlreadyGreen = _currentColor == FireColor.Green;

        if (instant)
        {
            Scale = new Vector2(target, target);
            if (isAlreadyGreen) return;
            _green.Visible = size == FireSize.Large;
            _green.Modulate = new Color(1, 1, 1);
            colorNode.Visible = size == FireSize.Small;
            colorNode.Modulate = new Color(1, 1, 1);
        }
        else
        {
            var tween = CreateTween().SetParallel();
            tween.TweenProperty(this, "scale", new Vector2(target, target), 0.3f)
                .SetTrans(Tween.TransitionType.Sine)
                .SetEase(Tween.EaseType.Out);

            if (isAlreadyGreen) return;
            var showNode = size == FireSize.Large ? _green : colorNode;
            var hideNode = size == FireSize.Large ? colorNode : _green;

            showNode.Modulate = new Color(1, 1, 1, 0);
            showNode.Visible = true;
            hideNode.Visible = true;

            tween.TweenProperty(showNode, "modulate:a", 1f, 0.3f);
            tween.TweenProperty(hideNode, "modulate:a", 0f, 0.3f);
            tween.Chain().TweenCallback(Callable.From(() => hideNode.Visible = false));
        }
    }

    public enum FireColor { Red, Green, Blue, Yellow, Pink, Orange }
}