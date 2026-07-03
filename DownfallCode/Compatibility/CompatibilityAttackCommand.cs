using System.Linq.Expressions;
using System.Reflection;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Compatibility;

public static class CompatibilityAttackCommand
{
    private static readonly Func<AttackCommand, CardModel, CardPlay?, AttackCommand> FromCard = Build();

    public static AttackCommand DownfallFromCard(this AttackCommand command, CardModel card, CardPlay? cardPlay)
        => FromCard(command, card, cardPlay);

    private static Func<AttackCommand, CardModel, CardPlay?, AttackCommand> Build()
    {
        const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
        var type = typeof(AttackCommand);

        // New API: FromCard(CardModel, CardPlay)
        var withPlay = type.GetMethod("FromCard", flags, null,
            [typeof(CardModel), typeof(CardPlay)], null);
        if (withPlay != null)
        {
            // Exact signature match — open instance delegate, no expression tree needed.
            return withPlay.CreateDelegate<Func<AttackCommand, CardModel, CardPlay?, AttackCommand>>();
        }

        // Old API (V107): FromCard(CardModel)
        var cardOnly = type.GetMethod("FromCard", flags, null, [typeof(CardModel)], null)
                       ?? throw new MissingMethodException("AttackCommand.FromCard not found in any known signature.");

        var cmd = Expression.Parameter(type, "command");
        var card = Expression.Parameter(typeof(CardModel), "card");
        var play = Expression.Parameter(typeof(CardPlay), "cardPlay"); // accepted, dropped

        var call = Expression.Call(cmd, cardOnly, card);
        return Expression.Lambda<Func<AttackCommand, CardModel, CardPlay?, AttackCommand>>(
            call, cmd, card, play).Compile();
    }
}