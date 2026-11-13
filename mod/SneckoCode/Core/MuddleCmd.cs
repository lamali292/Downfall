using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Snecko.SneckoCode.Core;

public static class MuddleCmd
{
    private static LocString MuddleSelectionPrompt => new("card_selection", "TO_MUDDLE");

    public static Task MuddleHandCards(PlayerChoiceContext ctx, CardModel card, bool lowerOnly = false)
    {
        var amount = card.DynamicVars["Muddle"].IntValue;
        return MuddleHandCards(ctx, card, amount, lowerOnly);
    }

    private static async Task MuddleHandCards(PlayerChoiceContext ctx, CardModel card, int amount,
        bool lowerOnly = false)
    {
        var prefs = new CardSelectorPrefs(MuddleSelectionPrompt, amount);
        var cards = await CardSelectCmd.FromHand(ctx, card.Owner, prefs, c => c != card && MuddleCore.CanMuddle(c), card);
        await Muddle(ctx, cards, lowerOnly);
    }

    public static async Task Muddle(PlayerChoiceContext ctx, IEnumerable<CardModel> cards,
        bool lowerOnly = false)
    {
        foreach (var cardModel in cards) await Muddle(ctx, cardModel, lowerOnly);
    }

    public static Task<CardModel?> Muddle(PlayerChoiceContext ctx, CardModel card, bool lowerOnly = false)
    {
        return MuddleCore.Muddle(ctx, card, lowerOnly);
    }
}
