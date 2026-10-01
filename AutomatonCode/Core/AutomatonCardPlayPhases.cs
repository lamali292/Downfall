using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Core;

/// <summary>Automaton's play phases: Encode cards express their effect through their Encodings before the play.</summary>
public sealed class AutomatonCardPlayPhases : ICardPlayPhases
{
    public static readonly AutomatonCardPlayPhases Instance = new();

    public async Task<bool> BeforePlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await EncodeOutcome.RunPlayEffect(card, ctx, cardPlay);
        return true;
    }
}
