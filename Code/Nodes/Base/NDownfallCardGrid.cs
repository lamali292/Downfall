using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.Code.Nodes.Base;

[GlobalClass]
public partial class NDownfallCardGrid : NCardGrid
{
    public override void _Ready()
    {
        ConnectSignals();
    }
}