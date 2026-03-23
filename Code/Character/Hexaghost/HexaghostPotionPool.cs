using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Hexaghost;

public class HexaghostPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => Hexaghost.CharacterId;
    public override Color LabOutlineColor => Hexaghost.Color;
}