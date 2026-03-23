using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Gremlins;

public class GremlinsRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Gremlins.CharacterId;
    public override Color LabOutlineColor => Gremlins.Color;
}