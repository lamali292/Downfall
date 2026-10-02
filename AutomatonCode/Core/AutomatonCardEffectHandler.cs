using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Core;

/// <summary>Automaton's global card-play listener, registered on <see cref="Downfall.DownfallCode.Utils.CardExecutionHooks"/>.</summary>
public static class AutomatonCardEffectHandler
{
    /// <summary>Encode cards express their effect through their Encodings before the card's own effect runs.</summary>
    public static async Task<bool> DoBeforeOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await EncodeOutcome.RunPlayEffect(card, ctx, cardPlay);
        return true;
    }
}
