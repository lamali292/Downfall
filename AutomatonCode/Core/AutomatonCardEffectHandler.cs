using Automaton.AutomatonCode.Enchantments;
using Automaton.AutomatonCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Core;

public static class AutomatonCardEffectHandler
{
    public static async Task DoAfterOnPlayInternal(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is IEncodable encodable)
            await encodable.PlayEncodableEffect(ctx, cardPlay, EncodeContext.Direct);
        if (AutomatonCmd.IsEncodable(card) && card.Enchantment is not Encoding)
            await AutomatonCmd.EncodeCard(card, ctx);
    }
}