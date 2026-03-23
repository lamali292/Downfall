using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.SlimeBoss;

public class SlimeBossRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => SlimeBoss.CharacterId;
    public override Color LabOutlineColor => SlimeBoss.Color;
}