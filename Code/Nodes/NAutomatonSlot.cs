using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

public partial class NAutomatonSlot : Control
{
	private Control?         _visualParent;
	private NGridCardHolder? _holder;
	
	// Accurate global center regardless of parent scaling or HBox layout
	public Vector2 CardAnchorGlobal => GetGlobalTransform().Origin + (Size * GetGlobalTransform().Scale / 2f);
	
	private float _baseY;

	public float BobOffset
	{
		set
		{
			if (_visualParent != null)
				_visualParent.Position = _visualParent.Position with { Y = _baseY + value };
		}
	}

	public override void _Ready()
	{
		_visualParent = GetNode<Control>("VisualParent");
		_baseY = _visualParent.Position.Y;
	}

	public NGridCardHolder? SetCard(NCard cardNode)
	{
		ClearCard();

		_holder = NGridCardHolder.Create(cardNode);
		if (_holder == null) return null;

		_visualParent!.AddChild(_holder);

		Callable.From(() =>
		{
			if (_holder == null || _visualParent == null) return;
			_holder.Position = _visualParent.Size / 2f - _holder.Size / 2f;
		}).CallDeferred();

		return _holder;
	}

	public void ClearCard()
	{
		_holder?.QueueFree();
		_holder = null;
	}
}
