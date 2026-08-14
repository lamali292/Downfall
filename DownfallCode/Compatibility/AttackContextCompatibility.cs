using System.Linq.Expressions;
using System.Reflection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands.Builders;   // AttackCommand + AttackContext
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Compatibility;

public static class AttackContextCompatibility
{
    private static readonly CreateContextDel CreateContextImpl = BuildCreateContext();

    public static Task<AttackContext> CreateContextAsync(
        ICombatState combatState,
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay)
    {
        return CreateContextImpl(combatState, choiceContext, cardPlay);
    }

    private static CreateContextDel BuildCreateContext()
    {
        // The method lives on AttackCommand, not AttackContext.
        var declaringType = typeof(AttackCommand);

        var newMethod = declaringType.GetMethod("CreateContextAsync",
            BindingFlags.Public | BindingFlags.Static, null,
            [typeof(ICombatState), typeof(PlayerChoiceContext), typeof(CardPlay)], null);

        var oldMethod = declaringType.GetMethod("CreateContextAsync",
            BindingFlags.Public | BindingFlags.Static, null,
            [typeof(ICombatState), typeof(PlayerChoiceContext), typeof(CardModel)], null);

        var combatState = Expression.Parameter(typeof(ICombatState), "combatState");
        var choiceContext = Expression.Parameter(typeof(PlayerChoiceContext), "choiceContext");
        var cardPlay = Expression.Parameter(typeof(CardPlay), "cardPlay");

        Expression call;
        if (newMethod != null)
        {
            call = Expression.Call(newMethod, combatState, choiceContext, cardPlay);
        }
        else if (oldMethod != null)
        {
            var cardProp = typeof(CardPlay).GetProperty("Card")
                           ?? throw new MissingMemberException("CardPlay.Card not found");
            var card = Expression.Property(cardPlay, cardProp);
            call = Expression.Call(oldMethod, combatState, choiceContext, card);
        }
        else
        {
            var found = string.Join(" | ", declaringType
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name == "CreateContextAsync")
                .Select(m => $"({string.Join(",", m.GetParameters().Select(p => p.ParameterType.Name))})"));
            throw new MissingMethodException(
                $"AttackCommand.CreateContextAsync not found. Overloads present: {found}");
        }

        return Expression.Lambda<CreateContextDel>(call, combatState, choiceContext, cardPlay).Compile();
    }

    private delegate Task<AttackContext> CreateContextDel(
        ICombatState combatState, PlayerChoiceContext choiceContext, CardPlay cardPlay);
}