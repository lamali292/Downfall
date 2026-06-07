using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Interfaces;

public interface IEncodable
{
    LocString EncodeLocString => (this is CardModel card ? BuildEncodeLocString(card) : null) ?? throw new Exception();

    Task PlayEncodableEffect(PlayerChoiceContext ctx, CardPlay cardPlay, EncodeContext encodeContext)
    {
        return Task.CompletedTask;
    }

    static LocString BuildEncodeLocString(CardModel card)
    {
        var loc = new LocString("encode", card.Id.Entry + ".encode");
        card.DynamicVars.AddTo(loc);
        return loc;
    }

    LocString GetEncodeLocString(EncodeContext context)
    {
        return EncodeLocString;
    }
}