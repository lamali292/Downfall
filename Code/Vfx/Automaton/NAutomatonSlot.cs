using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.Code.Nodes;

[GlobalClass]
public partial class NAutomatonSlot : Control
{
    private float _baseY;
    private NCustomCardHolder? _holder;
    private Control? _visualParent;

    public Vector2 CardAnchorGlobal => GetGlobalTransform().Origin + Size * GetGlobalTransform().Scale / 2f;

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

    public NCustomCardHolder? SetCard(NCard cardNode)
    {
        ClearCard();

        _holder = NCustomCardHolder.Create(cardNode, 1.0f, 2.0f);
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