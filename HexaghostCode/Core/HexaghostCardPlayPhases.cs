using Downfall.DownfallCode.Utils;
using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Core;

/// <summary>Hexaghost's play phases: Retract before the card's effect, Advance after it.</summary>
public sealed class HexaghostCardPlayPhases : ICardPlayPhases
{
    public static readonly HexaghostCardPlayPhases Instance = new();

    public async Task<bool> BeforePlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var retract = cardPlay.Card.Keywords.Contains(HexaghostKeyword.Retract);
        if (retract) await HexaghostCmd.Retract(ctx, cardPlay.Card.Owner, cardPlay.Card);
        return true;
    }

    public async Task AfterPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var advance = cardPlay.Card.Keywords.Contains(HexaghostKeyword.Advance);
        if (advance) await HexaghostCmd.Advance(ctx, cardPlay.Card.Owner, cardPlay.Card);
    }
}
