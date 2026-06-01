using Collector.CollectorCode.Core;
using Downfall.DownfallCode.Abstract;
using Godot;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class NCollectorEnergyCounter : Control
{
	private Tween? _fadeTween;
	private Label? _label;
	private Player? _player;
	private CollectorEnergy? _resource;
	private Control? _rotationLayers;

	public void Initialize(Player player)
	{
		_player = player;

		_resource = CardResourceRegistry.Get<CollectorEnergy>();

		if (_resource != null)
			_resource.Changed += OnEnergyChanged;
	}

	public override void _Ready()
	{
		_rotationLayers = GetNode<Control>("%RotationLayers");
		_label = GetNode<Label>("%Label");

		Visible = false;
		Refresh();
	}

	public override void _ExitTree()
	{
		if (_resource != null)
			_resource.Changed -= OnEnergyChanged;
	}

	public override void _Process(double delta)
	{
		if (_rotationLayers == null) return;
		for (var i = 0; i < _rotationLayers.GetChildCount(); i++)
			_rotationLayers.GetChild<Control>(i).RotationDegrees += (float)delta * 30f * (i + 1);
	}

	private void OnEnergyChanged(Player player, int amount)
	{
		if (player != _player) return;
		Refresh();
	}

	private void Refresh()
	{
		if (_player == null || _label == null || _resource == null) return;
		var amount = _resource.Get(_player);

		_label.Text = amount.ToString();
		_label.AddThemeColorOverride("font_color",
			amount == 0 ? StsColors.red : new Color("EBFFAD"));
		_label.AddThemeColorOverride("font_outline_color",
			amount == 0 ? StsColors.unplayableEnergyCostOutline : new Color("3A3F2B"));

		var targetAlpha = amount > 0 ? 1f : 0f;

		if (!Visible && amount > 0)
		{
			Visible = true;
			Modulate = new Color(1, 1, 1, 0f);
		}

		_fadeTween?.Kill();
		_fadeTween = CreateTween();
		_fadeTween.TweenProperty(this, "modulate:a", targetAlpha, 0.3)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Cubic);

		if (amount == 0)
			_fadeTween.TweenCallback(Callable.From(() => Visible = false));
	}

	public static NCollectorEnergyCounter Create(Player player)
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Collector/scenes/ui/collector_energy.tscn");
		var instance = scene.Instantiate<NCollectorEnergyCounter>();
		instance.Initialize(player);
		return instance;
	}
}
