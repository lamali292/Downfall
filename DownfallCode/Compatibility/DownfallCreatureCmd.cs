using System.Linq.Expressions;
using System.Reflection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Compatibility;

public static class DownfallCreatureCmd
{
    // --- public API, identical on both game versions ---

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        => SingleWithDealer(choiceContext, target, amount, props, dealer, cardSource, cardPlay);

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext, Creature target, decimal amount,
        ValueProp props, CardModel cardSource, CardPlay? cardPlay)
        => SingleCardOnly(choiceContext, target, amount, props, cardSource, cardPlay);

    public static Task<IEnumerable<DamageResult>> Damage(
        PlayerChoiceContext choiceContext, IEnumerable<Creature> targets, decimal amount,
        ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
        => MultiWithDealer(choiceContext, targets, amount, props, dealer, cardSource, cardPlay);

    // --- plumbing ---

    private delegate Task<IEnumerable<DamageResult>> SingleDealerDel(
        PlayerChoiceContext ctx, Creature target, decimal amount,
        ValueProp props, Creature? dealer, CardModel? card, CardPlay? play);

    private delegate Task<IEnumerable<DamageResult>> SingleCardDel(
        PlayerChoiceContext ctx, Creature target, decimal amount,
        ValueProp props, CardModel card, CardPlay? play);

    private delegate Task<IEnumerable<DamageResult>> MultiDealerDel(
        PlayerChoiceContext ctx, IEnumerable<Creature> targets, decimal amount,
        ValueProp props, Creature? dealer, CardModel? card, CardPlay? play);

    private static readonly SingleDealerDel SingleWithDealer = Build<SingleDealerDel>(
        typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
        typeof(ValueProp), typeof(Creature), typeof(CardModel));

    private static readonly SingleCardDel SingleCardOnly = Build<SingleCardDel>(
        typeof(PlayerChoiceContext), typeof(Creature), typeof(decimal),
        typeof(ValueProp), typeof(CardModel));

    private static readonly MultiDealerDel MultiWithDealer = Build<MultiDealerDel>(
        typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal),
        typeof(ValueProp), typeof(Creature), typeof(CardModel));

    /// <summary>
    /// Finds CreatureCmd.Damage matching baseParams (optionally + CardPlay) and
    /// compiles a delegate whose signature always includes the CardPlay parameter.
    /// On old versions the CardPlay argument is simply dropped.
    /// </summary>
    private static TDelegate Build<TDelegate>(params Type[] baseParams) where TDelegate : Delegate
    {
        var withPlay = baseParams.Append(typeof(CardPlay)).ToArray();

        var method = typeof(CreatureCmd).GetMethod("Damage",
                         BindingFlags.Public | BindingFlags.Static, null, withPlay, null)
                     ?? typeof(CreatureCmd).GetMethod("Damage",
                         BindingFlags.Public | BindingFlags.Static, null, baseParams, null)
                     ?? throw new MissingMethodException(
                         $"CreatureCmd.Damage({string.Join(", ", baseParams.Select(t => t.Name))}) not found");

        bool hasPlay = method.GetParameters().Length == withPlay.Length;

        // Lambda parameters always follow the *new* signature (incl. CardPlay).
        var lambdaParams = withPlay.Select((t, i) => Expression.Parameter(t, $"p{i}")).ToArray();
        var callArgs = hasPlay ? lambdaParams : lambdaParams[..^1];

        var call = Expression.Call(method, callArgs.Cast<Expression>());
        return Expression.Lambda<TDelegate>(call, lambdaParams).Compile();
    }
}