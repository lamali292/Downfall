using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Champ;

public class ChampPotionPool : CustomPotionPoolModel
{
    public override string EnergyColorName => Champ.CharacterId;
    public override Color LabOutlineColor => Champ.Color;
}