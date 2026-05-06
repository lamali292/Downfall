using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Automaton.AutomatonCode.Vfx;

[GlobalClass]
public partial class NAutomatonMerchantCharacter : NMerchantCharacter
{
    public override void _Ready() => PlayAnimation("idle", true);
}