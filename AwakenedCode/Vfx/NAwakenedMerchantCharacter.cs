using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Awakened.AwakenedCode.Vfx;

[GlobalClass]
public partial class NAwakenedMerchantCharacter : NMerchantCharacter
{
    public override void _Ready() => PlayAnimation("Idle_1", true);
}