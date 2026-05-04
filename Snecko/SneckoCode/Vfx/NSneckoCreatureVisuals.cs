using Godot;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Snecko.SneckoCode.Vfx;

[GlobalClass]
public partial class NSneckoCreatureVisuals : NCreatureVisuals
{
	public override void _Ready()
	{
		base._Ready();
		var premultMat = new CanvasItemMaterial
		{
			BlendMode = CanvasItemMaterial.BlendModeEnum.PremultAlpha
		};

		if (SpineBody != null)
			SpineBody.SetNormalMaterial(premultMat);
		else
			GetCurrentBody().Material = premultMat;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
}