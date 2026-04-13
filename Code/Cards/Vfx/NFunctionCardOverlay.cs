using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.Code.Cards.Vfx;

public partial class NFunctionCardOverlay : Control
{
	[Export] public ShaderMaterial? GlitchMaterial;

	public override void _Ready()
	{
		MouseFilter = MouseFilterEnum.Ignore;

		_material = Material as ShaderMaterial;
		Material = null;

		var parent = GetParent();
		while (parent != null && parent is not NCard)
			parent = parent.GetParent();

		if (parent is not NCard card) return;
			
		_card = card;
		
		var portrait = card.GetNode<TextureRect>("%Frame");
		if (portrait == null || _material == null) return;
		
		
		_portrait = portrait;
		_portrait.Material = _material;
	}
	private ShaderMaterial? _material;
	
	public override void _ExitTree()
	{
		if (_portrait != null && IsInstanceValid(_portrait))
			_portrait.Material = null;
	}

	private NCard? _card;
	private TextureRect? _portrait;
}
