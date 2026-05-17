using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace SlimeBoss.SlimeBossCode.Vfx;

[GlobalClass]
public partial class NSlimeBossMerchantCharacter : NMerchantCharacter
{
    public override void _Ready()
    {
        PlayAnimation("idle", true);
    }
}