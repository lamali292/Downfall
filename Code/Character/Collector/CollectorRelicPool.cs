using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Collector;

public class CollectorRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Collector.CharacterId;
    public override Color LabOutlineColor => Collector.Color;
}