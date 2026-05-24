using Automaton.AutomatonCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Interfaces;

public interface IEncodable
{
    LocString? EncodeLocString => this is CardModel card ? BuildEncodeLocString(card) : null;

    bool AutoEncode => true;

    async Task Encode(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (this is CardModel card)
        {
            await AutomatonCmd.EncodeCard(card, ctx, cardPlay);
            await Cmd.Wait(0.2f);
        }
    }

    Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        return Task.CompletedTask;
    }

    static LocString? BuildEncodeLocString(CardModel card)
    {
        var key = card.Id.Entry + ".encode";
        if (!LocString.Exists("encode", key)) return null;
        var loc = new LocString("encode", key);
        card.DynamicVars.AddTo(loc);
        return loc;
    }

    LocString? GetEncodeLocString(EncodeContext context)
    {
        return EncodeLocString;
    }
}