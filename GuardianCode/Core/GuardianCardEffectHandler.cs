using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Core;

public static class GuardianCardEffectHandler
{
    public static async Task DoAfterOnPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is not IGemSocketCard guardianCardModel) return;
        foreach (var gem in guardianCardModel.Gems)
            await gem.OnPlayWrapper(ctx, cardPlay, guardianCardModel.GemReplayCount);
    }
}