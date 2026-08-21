using Automaton.AutomatonCode.Cards.Token;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Events;

public static class AutomatonHook
{
    public static Task OnCardEncoded(ICombatState cs, PlayerChoiceContext ctx, CardModel card)
    {
        return HookUtils.Dispatch<IOnEncode>(cs, ctx, m => m.OnCardEncoded(ctx, card));
    }

    public static Task AfterCardsStashed(ICombatState? cs,  PlayerChoiceContext ctx, Player player, IEnumerable<CardModel> stashedCards, IEnumerable<CardModel> overflowCards)
    {
        return HookUtils.Dispatch<IAfterCardStashed>(cs, ctx, m => m.AfterCardsStashed(ctx, player, stashedCards, overflowCards));
    }

    public static int ModifyStashDraw(ICombatState cs, int orignal, Player player,
        out IEnumerable<IModifyStashDraw> modifiers)
    {
        return HookUtils.Modify(cs, orignal, (e, amount) => e.ModifyStashDraw(amount, player), out modifiers);
    }


    public static FunctionCard ModifyCompiledFunction(ICombatState cs, FunctionCard original, Player player,
        out IEnumerable<IModifyCompiledFunction> modifiers)
    {
        return HookUtils.ModifyMutable(cs, original, (e, amount) => e.ModifyCompiledFunction(amount, player),
            out modifiers);
    }

    public static Task AfterModifyCompiledFunction(ICombatState cs, IEnumerable<IModifyCompiledFunction> modifiers,
        Player player, FunctionCard result)
    {
        return HookUtils.AfterModifying(cs, modifiers, m => m.AfterModifyCompiledFunction(result, player));
    }

    public static Task AfterCompilingFunction(PlayerChoiceContext ctx, ICombatState cs, Player player,
        CardPileAddResult result)
    {
        return HookUtils.Dispatch<IAfterCompilingFunction>(cs, ctx,
            m => m.AfterCompilingFunction(ctx, player, result));
    }
}

