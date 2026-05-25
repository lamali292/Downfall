using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;

namespace Automaton.AutomatonCode.Powers;

public class TerminatorPower : AutomatonPowerModel, IModifyCompiledFunction
{
    public TerminatorPower()
    {
        WithTip(StaticHoverTip.ReplayStatic);
    }

    public bool ModifyCompiledFunction(FunctionCard function, Player player)
    {
        if (player.Creature != Owner) return false;
        function.BaseReplayCount += 1;
        return true;
    }

    public Task AfterModifyCompiledFunction(FunctionCard result, Player player)
    {
        return PowerCmd.Decrement(this);
    }
}