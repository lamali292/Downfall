using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Interfaces;

public interface IEncodable
{
    LocString? EncodeLocString => this is CardModel card ? BuildEncodeLocString(card) : null;

    bool AutoEncode => true;

    // Default implementation of the logic
    async Task Encode(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        // 'this' refers to the object implementing the interface
        if (this is CardModel card)
        {
            await OnEncode(ctx, cardPlay);
            await AutomatonCmd.EncodeCard(card, ctx, cardPlay);
            await Cmd.Wait(0.2f);
        }
    }

    // An optional hook for specific cards to do something unique during encoding
    Task OnEncode(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
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