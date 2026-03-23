using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Collector;

public class CollectorPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => Collector.CharacterId;
    public override Color LabOutlineColor => Collector.Color;
}