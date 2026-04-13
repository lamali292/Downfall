using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Guardian;

public class GuardianRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Guardian.CharacterId;
    public override Color LabOutlineColor => Guardian.Color;
}