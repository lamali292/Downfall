using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Guardian.GuardianCode.Vfx;

[GlobalClass]
public partial class NGuardianMerchantCharacter : NMerchantCharacter
{
    public override void _Ready() => PlayAnimation("idle", true);
}