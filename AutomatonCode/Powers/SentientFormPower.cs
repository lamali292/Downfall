using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class SentientFormPower : AutomatonPowerModel, IModifyCompiledFunction
{
    public SentientFormPower()
    {
        WithTip(StaticHoverTip.ReplayStatic);
    }

    public override Task AfterPowerAmountChanged(PlayerChoiceContext choiceContext, PowerModel power, decimal amount, Creature? applier,
        CardModel? cardSource)
    {
        if (power != this || Owner.Player?.PlayerCombatState == null) return Task.CompletedTask;
        foreach (var function in Owner.Player.PlayerCombatState.AllCards.OfType<FunctionCard>())
        {
            function.BaseReplayCount += (int)amount;
        }
        return Task.CompletedTask;
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