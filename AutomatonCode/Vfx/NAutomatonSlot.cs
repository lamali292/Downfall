using Downfall.DownfallCode.Nodes;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Automaton.AutomatonCode.Vfx;

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

    public NCustomCardHolder? SetCard(NCard cardNode, float scale = 1.0f)
    {
        ClearCard();

        _holder = NCustomCardHolder.Create(cardNode, 1.0f, 1.2f);
        if (_holder == null) return null;

        _visualParent!.AddChild(_holder);

        Callable.From(() =>
        {
            if (_holder == null || !IsInstanceValid(_holder) || _visualParent == null) return;
            _holder.Position = _visualParent.Size / 2f - _holder.Size * _holder.Scale / 2f;
        }).CallDeferred();

        return _holder;
    }

    /// <summary>
    ///     Frees the holder, but never a card node inside it. The NCard is a pooled
    ///     node the base game may have adopted via FindOnTable; if it's still under
    ///     the holder we detach it first so QueueFreeing the holder can't destroy it.
    ///     Card ownership belongs to the display (ReleaseAllSlotCards) or the game —
    ///     never to this slot.
    /// </summary>
    public void ClearCard()
    {
        if (_holder != null && IsInstanceValid(_holder))
        {
            var cardNode = _holder.CardNode;
            if (cardNode != null && IsInstanceValid(cardNode)
                && cardNode.GetParent() != null && _holder.IsAncestorOf(cardNode))
            {
                cardNode.GetParent().RemoveChild(cardNode);
            }

            _holder.QueueFree();
        }

        _holder = null;
    }
}