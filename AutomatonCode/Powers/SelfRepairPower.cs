using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Rooms;

namespace Automaton.AutomatonCode.Powers;

public class SelfRepairPower : AutomatonPowerModel
{
    public override Task AfterCombatEnd(CombatRoom room)
        =>  CreatureCmd.Heal(Owner, Amount);
}