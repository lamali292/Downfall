using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Collector.CollectorCode.Vfx;

[GlobalClass]
public partial class NCollectorMerchantCharacter : NMerchantCharacter
{
    public override void _Ready() => PlayAnimation("idle", true);
}