using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.HoverTips;

namespace Automaton.AutomatonCode.Powers;

public class SentientFormPower : AutomatonPowerModel, IModifyCompiledFunction
{
    public SentientFormPower()
    {
        WithTip(StaticHoverTip.ReplayStatic);
    }
    
    public bool ModifyCompiledFunction(FunctionCard function, Player player)
    {
        if (player.Creature != Owner) return false;
        function.BaseReplayCount += Amount;
        return true;
    }

    public Task AfterModifyCompiledFunction(FunctionCard result, Player player)
    {
        Flash();
        return Task.CompletedTask;
    }
}