using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Champ;

public class ChampRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Champ.CharacterId;
    public override Color LabOutlineColor => Champ.Color;
}