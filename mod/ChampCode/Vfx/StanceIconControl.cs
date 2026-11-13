using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.HoverTips;

namespace Champ.ChampCode.Vfx;

public partial class StanceIconControl : NClickableControl
{
    private IHoverTip? _tip;

    public void SetTip(IHoverTip tip)
    {
        _tip = tip;
    }

    public override void _Ready()
    {
        ConnectSignals();
    }

    protected override void OnFocus()
    {
        if (_tip == null) return;
        NHoverTipSet.CreateAndShow(this, _tip)
            ?.SetGlobalPosition(GlobalPosition + new Vector2(0f, Size.Y + 20f));
    }

    protected override void OnUnfocus()
    {
        NHoverTipSet.Remove(this);
    }
}