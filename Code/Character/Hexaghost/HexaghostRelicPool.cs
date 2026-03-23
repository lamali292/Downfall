using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Hexaghost;

public class HexaghostRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Hexaghost.CharacterId;
    public override Color LabOutlineColor => Hexaghost.Color;
}