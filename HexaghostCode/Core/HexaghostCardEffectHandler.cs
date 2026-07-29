using Hexaghost.HexaghostCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Core;

public class HexaghostCardEffectHandler
{
    public static async Task<bool> DoBeforeOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var retract = cardPlay.Card.Keywords.Contains(HexaghostKeyword.Retract);
        if (retract) await HexaghostCmd.Retract(ctx, cardPlay.Card.Owner, cardPlay.Card);
        return true;
    }

    public static async Task DoAfterOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var advance = cardPlay.Card.Keywords.Contains(HexaghostKeyword.Advance);
        if (advance) await HexaghostCmd.Advance(ctx, cardPlay.Card.Owner, cardPlay.Card);
    }
}