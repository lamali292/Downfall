using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Automaton.AutomatonCode.Powers;

public class FrontloadPower : AutomatonPowerModel, IModifyCompiledFunction
{
    public FrontloadPower()
    {
        WithTip(CardKeyword.Retain);
    }
    
    public bool ModifyCompiledFunction(FunctionCard function, Player player)
    {
        if (player.Creature != Owner) return false;
        function.AddKeyword(CardKeyword.Retain);
        return true;
    }

    public Task AfterModifyCompiledFunction(FunctionCard result, Player player)
      => PowerCmd.Decrement(this);
}