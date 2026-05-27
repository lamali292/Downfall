using BaseLib.Utils;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Core;

public static class HermitCardEffectHandler
{
    public static async Task<bool> DoBeforeOnPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card.Keywords.Contains(HermitKeywords.Concentrate))
            await CommonActions.ApplySelf<ConcentrationPower>(ctx, card, 1);
        return true;
    }

    public static async Task DoAfterOnPlay(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (card is IHasDeadOnEffect { IsDeadOn: true }) await HermitCmd.TriggerDeadOnEffect(ctx, card, cardPlay);
    }
}