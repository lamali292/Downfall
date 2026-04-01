using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Events;
using Downfall.Code.Interfaces;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Powers.Automaton;

public class MergePower : AutomatonPowerModel, IOnEncode
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;


    public async Task OnCardEncoded(PlayerChoiceContext ctx, CardModel encodedCard, CardPlay cardPlay)
    {
        if (encodedCard.Owner != Owner.Player) return;
        if (Amount <= 0) return;

        var copies = Amount;
        await PowerCmd.Remove(this);

        for (var i = 0; i < copies; i++)
        {
            var dupe = encodedCard.CreateClone();
            if (dupe is not IEncodable model) continue;
            await model.Encode(ctx, cardPlay);
        }
    }
}