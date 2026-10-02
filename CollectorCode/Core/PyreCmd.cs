using Collector.CollectorCode.Events;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Core;

public static class PyreCmd
{
    private static async Task PyreCards(PlayerChoiceContext ctx, CardModel card, IEnumerable<CardModel> pyred)
    {
        if (card.CombatState == null) return;
        foreach (var c in pyred)
        {
            if (CollectorHook.ShouldExhaustPyred(card, c))
            {
                await CardCmdCompatibility.Exhaust(ctx, c);
            }
            await CollectorHook.AfterCardPyred(card.CombatState, ctx, card, c);
            await Cmd.Wait(0.1f);
        }
    }

    public static async Task<CardModel?> Pyre(PlayerChoiceContext ctx, CardModel card)
    {
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.ExhaustSelectionPrompt, 1, 1);
        var pyred = (await CardSelectCmd.FromHand(ctx, card.Owner, prefs, e => e != card, card)).FirstOrDefault();
        if (pyred == null || card.CombatState == null) return pyred;
        await PyreCards(ctx, card, [pyred]);
        return pyred;
    }

    public static async Task<IReadOnlyList<CardModel>> MegaPyre(PlayerChoiceContext ctx, CardModel card)
    {
        if (card.CombatState == null) return [];
        var cards = card.Owner.Hand.ToList();
        await PyreCards(ctx, card, cards);
        return cards;
    }
}
