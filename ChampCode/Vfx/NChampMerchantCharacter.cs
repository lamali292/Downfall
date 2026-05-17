using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Shops;

namespace Champ.ChampCode.Vfx;

[GlobalClass]
public partial class NChampMerchantCharacter : NMerchantCharacter
{
    public override void _Ready()
    {
        PlayAnimation("Idle", true);
    }
}