using Downfall.DownfallCode.Nodes;
using Godot;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guardian.GuardianCode.Vfx;

[GlobalClass]
public partial class NStasisSlot : Control
{
    private float _baseY;
    private MegaLabel? _count;
    private NCustomCardHolder? _holder;

    private Control? _visualParent;
    public Vector2 CardAnchorGlobal => GetGlobalTransform().Origin + Size * GetGlobalTransform().Scale / 2f;

    private static float CardScale => 0.15f;
    private static float BigCardScale => 0.75f;

    public override void _Ready()
    {
        var anim = GetNode<AnimationPlayer>("AnimationPlayer");
        anim.Play("idle");
        anim.Seek((float)GD.RandRange(0, anim.CurrentAnimationLength), true);
        _visualParent = GetNode<Control>("%Visuals");
        _count = GetNode<MegaLabel>("%Count");
        //_cardHolder = GetNode<NCustomCardHolder>("%CardHolder");
        _count.Text = ""; // Clear default text
        _count.Visible = false;
        _baseY = _visualParent.Position.Y;
    }

    public NCustomCardHolder? SetCard(NCard cardNode)
    {
        ClearCard();

        _holder = NCustomCardHolder.Create(cardNode, CardScale, BigCardScale);
        if (_holder == null) return null;

        _visualParent!.AddChild(_holder);
        if (cardNode.Model != null)
            UpdateCounterDisplay(cardNode.Model);
        Callable.From(() =>
        {
            if (_holder == null || _visualParent == null) return;
            _holder.Position = _visualParent.Size / 2f - _holder.Size / 2f;
        }).CallDeferred();

        return _holder;
    }


    public void UpdateCounterDisplay(CardModel card)
    {
        if (_count == null) return;
        var counter = GuardianCmd.GetStasisCounter(card);
        if (counter > 0)
        {
            _count.Text = counter.ToString();
            _count.Visible = true;
        }
        else
        {
            _count.Visible = false;
        }
    }

    public void ClearCard()
    {
        _holder?.QueueFree();
        _holder = null;
        if (_count == null) return;
        _count.Visible = false;
    }
}