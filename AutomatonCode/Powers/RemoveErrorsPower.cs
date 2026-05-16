using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Displays;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class RemoveErrorsPower : AutomatonPowerModel, IOnEncode
{
    public async Task OnCardEncoded(PlayerChoiceContext ctx, CardModel encodedCard, CardPlay cardPlay)
    {
        if (encodedCard.Owner != Owner.Player) return;
        if (encodedCard is not (ICompilableError and AutomatonCardModel automatonCardModel)) return;
        automatonCardModel.SuppressCompileError = true;
        AutomatonDisplay.Refresh(Owner.Player);
        Flash();
        await PowerCmd.Decrement(this);
    }
}