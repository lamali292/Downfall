using System.Linq.Expressions;
using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Compatibility;

public static class CardCmdCompatibility
{
    private static readonly ExhaustDel ExhaustImpl = BuildExhaust();

    /// <summary>
    ///     Exhaust a card. Returns the CardPileAddResult on new game versions,
    ///     or null on old versions (which don't return one).
    /// </summary>
    public static Task<CardPileAddResult?> Exhaust(
        PlayerChoiceContext choiceContext,
        CardModel card,
        bool causedByEthereal = false,
        bool skipVisuals = false)
    {
        return ExhaustImpl(choiceContext, card, causedByEthereal, skipVisuals);
    }

    private static ExhaustDel BuildExhaust()
    {
        var paramTypes = new[]
        {
            typeof(PlayerChoiceContext), typeof(CardModel), typeof(bool), typeof(bool)
        };

        var method = typeof(CardCmd).GetMethod("Exhaust",
                         BindingFlags.Public | BindingFlags.Static, null, paramTypes, null)
                     ?? throw new MissingMethodException("CardCmd.Exhaust not found");

        var ctx = Expression.Parameter(typeof(PlayerChoiceContext), "ctx");
        var card = Expression.Parameter(typeof(CardModel), "card");
        var ethereal = Expression.Parameter(typeof(bool), "ethereal");
        var skipVisuals = Expression.Parameter(typeof(bool), "skipVisuals");

        var call = Expression.Call(method, ctx, card, ethereal, skipVisuals);

        // New version: returns Task<CardPileAddResult?> — use as-is.
        if (method.ReturnType == typeof(Task<CardPileAddResult?>))
            return Expression.Lambda<ExhaustDel>(call, ctx, card, ethereal, skipVisuals).Compile();

        // Old version: returns plain Task — wrap it so we still return Task<CardPileAddResult?> (null).
        if (typeof(Task).IsAssignableFrom(method.ReturnType))
        {
            // We can't easily Expression-compile the async wrap; delegate to a helper.
            var oldDel = Expression.Lambda<OldExhaustDel>(call, ctx, card, ethereal, skipVisuals).Compile();
            return async (c, cd, e, s) =>
            {
                await oldDel(c, cd, e, s);
                return null;
            };
        }

        throw new MissingMethodException(
            $"CardCmd.Exhaust has unexpected return type {method.ReturnType}");
    }

    private delegate Task<CardPileAddResult?> ExhaustDel(
        PlayerChoiceContext ctx, CardModel card, bool ethereal, bool skipVisuals);

    private delegate Task OldExhaustDel(
        PlayerChoiceContext ctx, CardModel card, bool ethereal, bool skipVisuals);
}