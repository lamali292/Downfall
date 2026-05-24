using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Automaton.AutomatonCode.Powers;

public class SpikePower : AutomatonPowerModel, IAfterCompilingFunction
{
    public async Task AfterCompilingFunction(PlayerChoiceContext ctx, Player player, CardPileAddResult result,
        CardPlay cardPlay)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<ThornsPower>(ctx, Owner, Amount, Owner, null);
        await PowerCmd.Remove(this);
    }
}