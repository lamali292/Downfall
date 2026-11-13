using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Automaton.AutomatonCode.Events;
using Automaton.AutomatonCode.Interfaces;

namespace Automaton.AutomatonCode.Core;

public static class EncodeOutcome
{
 

    /// <summary>Before the card's own effect: player-encodable cards express their effect through their Encodings.</summary>
    public static async Task EncodePlayEffect(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (!AutomatonCmd.IsEncodable(card)|| card is not IEncodable encodable) return;
        foreach (var encoding in encodable.Encodings)
            await encoding.OnPlay(card, ctx, cardPlay.Target, cardPlay);
    }
    
    public static async Task CommitAfterPlay(CardModel card, PlayerChoiceContext ctx)
    {
        if (AutomatonCmd.IsEncodable(card))
            await AutomatonCmd.EncodeCard(card, ctx);
    }
    
    public static CardLocationCompatiblity HideFromDiscard(CardModel card, CardLocationCompatiblity location)
    {
        if (location.PileType != PileType.Discard || !AutomatonCmd.IsEncodable(card)) return location;
        return location with { PileType = PileType.None };
    }
}
