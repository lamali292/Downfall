using Godot;

namespace Hexaghost.HexaghostCode.Vfx;

[GlobalClass]
public partial class NFire : Node2D
{
	public enum FireSize
	{
		Large,
		Small
	}

	private const float LargeScale = 0.5f;
	private const float SmallScale = 0.25f;
	private Node2D? _blue;

	private FireColor _currentColor = FireColor.Red;
	private Node2D? _green;
	private Node2D? _orange;
	private Node2D? _pink;
	private Node2D? _red;
	private Node2D? _yellow;

	public FireSize CurrentSize { get; private set; } = FireSize.Small;

	public override void _Ready()
	{
		_red = GetNode<Node2D>("%fire_red");
		_green = GetNode<Node2D>("%fire_green");
		_blue = GetNode<Node2D>("%fire_blue");
		_yellow = GetNode<Node2D>("%fire_yellow");
		_pink = GetNode<Node2D>("%fire_pink");
		_orange = GetNode<Node2D>("%fire_orange");
	}

	private Node2D? GetColorNode(FireColor color)
	{
		return color switch
		{
			FireColor.Red => _red,
			FireColor.Green => _green,
			FireColor.Blue => _blue,
			FireColor.Yellow => _yellow,
			FireColor.Pink => _pink,
			FireColor.Orange => _orange,
			_ => _red
		};
	}

	public void SetState(FireColor color, FireSize size, bool instant = false)
	{
		_currentColor = color;
		CurrentSize = size;

		var target = size == FireSize.Large ? LargeScale : SmallScale;
		var showNode = size == FireSize.Large && color != FireColor.Green ? _green : GetColorNode(color);
		var hideNodes = Enum.GetValues<FireColor>()
			.Select(GetColorNode)
			.Where(n => n != null && n != showNode)
			.ToList();

		if (showNode == null) return;

		if (instant)
		{
			Scale = new Vector2(target, target);
			showNode.Visible = true;
			showNode.Modulate = new Color(1, 1, 1);
			foreach (var n in hideNodes)
			{
				n!.Visible = false;
				n.Modulate = new Color(1, 1, 1);
			}
		}
		else
		{
			var tween = CreateTween().SetParallel();
			tween.TweenProperty(this, "scale", new Vector2(target, target), 0.3f)
				.SetTrans(Tween.TransitionType.Sine)
				.SetEase(Tween.EaseType.Out);

			showNode.Modulate = new Color(1, 1, 1, showNode.Visible ? 1 : 0);
			showNode.Visible = true;
			tween.TweenProperty(showNode, "modulate:a", 1f, 0.3f);

			foreach (var n in hideNodes.Where(n => n!.Visible))
			{
				tween.TweenProperty(n, "modulate:a", 0f, 0.3f);
				tween.Chain().TweenCallback(Callable.From(() => n!.Visible = false));
			}
		}
	}
}

public enum FireColor
{
	Red,
	Green,
	Blue,
	Yellow,
	Pink,
	Orange
}

public static class FireColorExtensions
{
	public static Color ToColor(this FireColor fireColor)
	{
		return fireColor switch
		{
			FireColor.Red => new Color(0xFF971BFF),
			FireColor.Green => new Color(0x97FF5FFF),
			FireColor.Blue => new Color(0x70FFFFFF),
			FireColor.Yellow => new Color(1f, 0.9f, 0.1f),
			FireColor.Pink => new Color(0xFF72FFFF),
			FireColor.Orange => new Color(1f, 0.5f, 0.1f),
			_ => Colors.White
		};
	}
}
