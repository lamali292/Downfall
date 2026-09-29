using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Core;

public static class AutomatonCardEffectHandler
{
    public static async Task<bool> DoBeforeOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await EncodeOutcome.RunPlayEffect(card, ctx, cardPlay);
        return true;
    }
}