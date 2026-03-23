using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Automaton;

public class RemoveErrorsPower : AutomatonPowerModel, IOnEncode
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public async Task OnCardEncoded(PlayerChoiceContext ctx, CardModel encodedCard, CardPlay cardPlay)
    {
        if (encodedCard.Owner != Owner.Player) return;
        if (encodedCard is not (ICompilableError and AutomatonCardModel automatonCardModel)) return;
        automatonCardModel.SuppressCompileError = true;
        AutomatonCmd.RefreshDisplay(Owner);
        Flash();
        await PowerCmd.Decrement(this);
    }
}