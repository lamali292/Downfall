using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Guardian;

public class GuardianPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => Guardian.CharacterId;
    public override Color LabOutlineColor => Guardian.Color;
}