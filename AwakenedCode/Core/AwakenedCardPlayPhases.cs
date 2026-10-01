using Awakened.AwakenedCode.Interfaces;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Core;

/// <summary>Awakened's play phases: a chantable card chants after it resolved.</summary>
public sealed class AwakenedCardPlayPhases : ICardPlayPhases
{
    public static readonly AwakenedCardPlayPhases Instance = new();

    public async Task AfterPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is IChantable chantable && (AwakenedCmd.WasLastCardPlayedPower(cardPlay) || chantable.HasChanted))
            await AwakenedCmd.Chant(ctx, card, cardPlay);
    }
}
