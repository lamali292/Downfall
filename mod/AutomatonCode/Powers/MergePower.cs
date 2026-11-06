using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Powers;

public class MergePower : AutomatonPowerModel, IOnEncode
{
    public async Task OnCardEncoded(PlayerChoiceContext ctx, CardModel encodedCard)
    {
        if (encodedCard.Owner != Owner.Player) return;
        if (Amount <= 0) return;

        var copies = Amount;
        await PowerCmd.Remove(this);

        for (var i = 0; i < copies; i++)
        {
            var dupe = encodedCard.CreateClone();
            await AutomatonCmd.EncodeCard(dupe, ctx);
        }
    }
}