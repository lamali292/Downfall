using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Snecko;

public class SneckoPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => Snecko.CharacterId;
    public override Color LabOutlineColor => Snecko.Color;
}