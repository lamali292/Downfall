using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Snecko;

public class SneckoRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Snecko.CharacterId;
    public override Color LabOutlineColor => Snecko.Color;
}