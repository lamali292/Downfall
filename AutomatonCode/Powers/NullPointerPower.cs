using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Automaton.AutomatonCode.Powers;

public class NullPointerPower : AutomatonPowerModel, IModifyCompiledFunction
{
    public NullPointerPower() :  base(PowerType.Debuff)
    {
        WithTip(CardKeyword.Unplayable);
    }
    
    public bool ModifyCompiledFunction(FunctionCard function, Player player)
    {
        if (player.Creature != Owner) return false;
        function.AddKeyword(CardKeyword.Unplayable);
        return true;
    }

    public Task AfterModifyCompiledFunction(FunctionCard result, Player player)
        => PowerCmd.Decrement(this);
}