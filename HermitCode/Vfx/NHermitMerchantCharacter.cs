using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Hermit.HermitCode.Vfx;

[GlobalClass]
public partial class NHermitMerchantCharacter : NMerchantCharacter
{
    public override void _Ready()
    {
        PlayAnimation("Idle", true);
    }
}