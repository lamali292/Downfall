using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Snecko.SneckoCode.Vfx;

[GlobalClass]
public partial class NSneckoMerchantCharacter : NMerchantCharacter
{
    public override void _Ready()
    {
        // Todo : fix black outlines
        PlayAnimation("Idle", true);
    }
}