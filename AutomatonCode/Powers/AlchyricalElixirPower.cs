using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Automaton.AutomatonCode.Powers;

public class AlchyricalElixirPower : AutomatonPowerModel, IModifyCompiledFunction
{
    public bool ModifyCompiledFunction(FunctionCard function, Player player)
    {
        if (player.Creature != Owner) return false;
        function.SetToFreeThisCombat();
        return true;
    }

    public Task AfterModifyCompiledFunction(FunctionCard result, Player player)
    {
        return PowerCmd.Decrement(this);
    }
}