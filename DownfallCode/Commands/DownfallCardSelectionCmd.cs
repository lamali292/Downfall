using BaseLib.Commands;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Commands;

/// <summary>
///     Player-facing card selection prompts (hand, a combat pile, an arbitrary card list, or
///     several piles at once) — thin wrappers over the base game's <see cref="CardSelectCmd" />
///     and BaseLib's <see cref="MultiPileCardSelect" /> that apply our default prefs. Split out
///     of <see cref="DownfallCardCmd" />, which owns card generation instead.
/// </summary>
public class DownfallCardSelectionCmd
{
    /// <summary>
    ///     Select from given cards with count manually specified.
    /// </summary>
    public static Task<IEnumerable<CardModel>> SelectFromCards(PlayerChoiceContext ctx,
        IReadOnlyList<CardModel> cards, LocString prompt, int count, CardModel cardSource,
        bool optional = false)
    {
        return CardSelectCmd.FromSimpleGrid(ctx, cards, cardSource.Owner, Prefs(prompt, count, optional));
    }

    /// <summary>
    ///     Select from given cards with count determined by <c>DynamicVars.Cards</c> or a default value of 1.
    /// </summary>
    public static Task<IEnumerable<CardModel>> SelectFromCards(PlayerChoiceContext ctx,
        IReadOnlyList<CardModel> cards, LocString prompt, CardModel cardSource,
        bool optional = false)
    {
        return SelectFromCards(ctx, cards, prompt, GetCardCount(cardSource), cardSource, optional);
    }

    public static Task<IEnumerable<CardModel>> SelectFromCombatPile(PlayerChoiceContext ctx,
        CardPile pile, LocString prompt, int count, CardModel cardSource, Func<CardModel, bool>? filter = null,
        bool optional = false)
    {
        return CardSelectCmd.FromCombatPile(ctx, pile, cardSource.Owner, Prefs(prompt, count, optional),
            filter ?? (_ => true));
    }

    public static Task<IEnumerable<CardModel>> SelectFromCombatPile(PlayerChoiceContext ctx,
        CardPile pile, LocString prompt, CardModel cardSource, Func<CardModel, bool>? filter = null,
        bool optional = false)
    {
        return SelectFromCombatPile(ctx, pile, prompt, GetCardCount(cardSource), cardSource, filter, optional);
    }

    /// <summary>
    ///     Select cards from hand with count manually specified.
    /// </summary>
    public static Task<IEnumerable<CardModel>> SelectFromHand(PlayerChoiceContext ctx, LocString prompt,
        int count, AbstractModel source,
        Func<CardModel, bool>? filter = null, bool optional = false)
    {
        return CardSelectCmd.FromHand(ctx, source.Creature.Player!, Prefs(prompt, count, optional), filter, source);
    }

    /// <summary>
    ///     Select cards from hand with count determined by <c>DynamicVars.Cards</c> or a default value of 1.
    /// </summary>
    public static Task<IEnumerable<CardModel>> SelectFromHand(PlayerChoiceContext ctx, LocString prompt,
        CardModel cardSource,
        Func<CardModel, bool>? filter = null, bool optional = false)
    {
        return SelectFromHand(ctx, prompt, GetCardCount(cardSource), cardSource, filter, optional);
    }

    /// <summary>
    ///     Select cards from hand with count determined by <c>Amount</c>.
    /// </summary>
    public static Task<IEnumerable<CardModel>> SelectFromHand(PlayerChoiceContext ctx, LocString prompt,
        PowerModel powerSource,
        Func<CardModel, bool>? filter = null, bool optional = false)
    {
        return SelectFromHand(ctx, prompt, powerSource.Amount, powerSource, filter, optional);
    }

    public static async Task<IEnumerable<CardModel>> MulitPileSelect(
        PlayerChoiceContext ctx,
        Player player,
        CardSelectorPrefs prefs,
        List<CardModel> cards,
        PileType[]? pileTypes = null)
    {
        if (!TestMode.IsOn)
            return await MultiPileCardSelect.Select(
                ctx,
                player,
                prefs,
                cards,
                pileTypes);
        if (CardSelectCmd.Selector == null)
            return [];

        return await CardSelectCmd.Selector.GetSelectedCards(
            cards,
            prefs.MinSelect,
            prefs.MaxSelect);
    }

    public static async Task<IEnumerable<CardModel>> MulitPileSelect(
        PlayerChoiceContext ctx,
        Player player,
        CardSelectorPrefs prefs,
        Func<CardModel, bool>? filter = null,
        params PileType[] pileTypes)
    {
        if (!TestMode.IsOn)
            return await MultiPileCardSelect.Select(
                ctx,
                player,
                prefs,
                filter,
                pileTypes);
        if (CardSelectCmd.Selector == null)
            return [];

        return await CardSelectCmd.Selector.GetSelectedCards(
            pileTypes.SelectMany(e => e.GetPile(player).Cards),
            prefs.MinSelect,
            prefs.MaxSelect);
    }

    private static int GetCardCount(CardModel cardSource)
    {
        return cardSource.DynamicVars.ContainsKey("Cards") ? cardSource.DynamicVars.Cards.IntValue : 1;
    }

    private static CardSelectorPrefs Prefs(LocString prompt, int count, bool optional)
    {
        return new CardSelectorPrefs(prompt, optional ? 0 : count, count);
    }
}
