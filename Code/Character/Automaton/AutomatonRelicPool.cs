using BaseLib.Abstracts;
using Godot;

namespace Downfall.Code.Character.Automaton;

public class AutomatonRelicPool : CustomRelicPoolModel
{
    public override string EnergyColorName => Automaton.CharacterId;
    public override Color LabOutlineColor => Automaton.Color;
}