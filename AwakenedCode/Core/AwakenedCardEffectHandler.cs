using Awakened.AwakenedCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Core;

public static class AwakenedCardEffectHandler
{
    public static async Task DoAfterOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is IChantable chantable && (AwakenedCmd.WasLastCardPlayedPower(cardPlay) || chantable.HasChanted))
            await AwakenedCmd.Chant(ctx, card, cardPlay);
    }
}