using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Combat;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Vfx;

/// <summary>
/// Small badge in the bottom-right corner of a slime showing what its next Command will do
/// (<see cref="SlimeModel.CounterValue"/>: damage, or the secondary value for block/thorns slimes),
/// including Potency and other modifiers. Green when above the slime's base value, red when below.
/// </summary>
/// <remarks>
/// The value is re-read a few times a second, but the label is only touched (and popped) when it actually
/// changed: it depends on arbitrary damage modifiers (powers, relics, other mods) and combat history,
/// which have no single change event to subscribe to.
/// </remarks>
public partial class NSlimeCounter : Control
{
	private const string ScenePath = "ui/slime_counter.tscn";
	private const float RefreshSeconds = 0.2f;
	/// <summary>Where the badge's top-left sits relative to the slime hitbox's bottom-right corner.</summary>
	private static readonly Vector2 CornerOffset = new(-19f, -3f);
	private static readonly Color Higher = new(0.5f, 1f, 0.5f);
	private static readonly Color Lower = new(1f, 0.45f, 0.45f);

	private NCreature _creatureNode = null!;
	private SlimeModel _slime = null!;
	private Control _content = null!;
	private TextureRect _icon = null!;
	private MegaRichTextLabel _value = null!;
	private int? _shownValue;
	private int _shownHits = 1;
	private string? _shownIcon;
	private Tween? _popTween;
	private float _sinceRefresh;

	public static NSlimeCounter Create(NCreature creatureNode, SlimeModel slime)
	{
		var counter = GD.Load<PackedScene>(ScenePath.SlimeScenePath()).Instantiate<NSlimeCounter>();
		counter._creatureNode = creatureNode;
		counter._slime = slime;
		return counter;
	}

	public override void _Ready()
	{
		_content = GetNode<Control>("%Content");
		_icon = GetNode<TextureRect>("%Icon");
		_value = GetNode<MegaRichTextLabel>("%Value");
		Refresh();
	}

	public override void _Process(double delta)
	{
		_sinceRefresh += (float)delta;
		if (_sinceRefresh < RefreshSeconds) return;
		_sinceRefresh = 0f;
		Refresh();
	}

	private void Refresh()
	{
		var alive = _slime.Creature.IsAlive;
		var statusIcon = alive ? _slime.CounterStatusIconPath : null;
		SetIcon(statusIcon ?? _slime.CounterIconPath);
		if (statusIcon != null)
		{
			// Status phase (e.g. asleep): icon only, no number.
			Visible = true;
			_shownValue = null;
			_value.Text = "";
			PlaceInCorner();
			return;
		}

		var shown = alive ? _slime.CounterValue : null;
		Visible = shown != null;
		if (shown is not { } value) return;

		var baseValue = _slime.CounterBaseValue;
		_value.Modulate = value > baseValue ? Higher : value < baseValue ? Lower : Colors.White;
		var hits = Math.Max(1, _slime.CounterHitCount);
		if ((int)value != _shownValue || hits != _shownHits)
		{
			var isFirst = _shownValue == null;
			_shownValue = (int)value;
			_shownHits = hits;
			// Mirrors intents.json FORMAT_DAMAGE_MULTI: "{Damage}[font_size=18]x{Repeat}[/font_size]".
			_value.Text = hits > 1 ? $"{_shownValue}[font_size=18]x{hits}[/font_size]" : _shownValue.ToString()!;
			if (!isFirst) Pop();
		}

		PlaceInCorner();
	}

	private void SetIcon(string path)
	{
		if (path == _shownIcon) return;
		var isFirst = _shownIcon == null;
		_shownIcon = path;
		_icon.Texture = GD.Load<Texture2D>(path);
		if (!isFirst) Pop();
	}

	private void PlaceInCorner()
	{
		var hitbox = _creatureNode.Hitbox;
		// The slime node (and so its hitbox) can be scaled. The badge moves with it but keeps its own size.
		var scale = hitbox.GetGlobalTransform().Scale;
		Scale = Vector2.One / scale;
		GlobalPosition = hitbox.GlobalPosition + (hitbox.Size + CornerOffset) * scale;
	}

	/// <summary>Brief scale-up of the effect counter around its centre, so a changed value catches the eye.</summary>
	private void Pop()
	{
		_content.PivotOffset = _content.Size / 2;
		_popTween?.Kill();
		_popTween = CreateTween();
		_popTween.TweenProperty(_content, "scale", Vector2.One, 0.3).From(Vector2.One * 1.5f)
			.SetTrans(Tween.TransitionType.Back).SetEase(Tween.EaseType.Out);
	}
}
