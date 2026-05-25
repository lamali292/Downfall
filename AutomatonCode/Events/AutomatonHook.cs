using Automaton.AutomatonCode.Cards.Token;
using Downfall.DownfallCode.Events;
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
        return DownfallHook.Dispatch<IOnEncode>(cs, ctx, m => m.OnCardEncoded(ctx, card));
    }


    public static int ModifyStashDraw(ICombatState cs, int orignal, Player player,
        out IEnumerable<IModifyStashDraw> modifiers)
    {
        return DownfallHook.Modify(cs, orignal, (e, amount) => e.ModifyStashDraw(amount, player), out modifiers);
    }


    public static FunctionCard ModifyCompiledFunction(ICombatState cs, FunctionCard original, Player player,
        out IEnumerable<IModifyCompiledFunction> modifiers)
    {
        return DownfallHook.ModifyMutable(cs, original, (e, amount) => e.ModifyCompiledFunction(amount, player),
            out modifiers);
    }

    public static Task AfterModifyCompiledFunction(ICombatState cs, IEnumerable<IModifyCompiledFunction> modifiers,
        Player player, FunctionCard result)
    {
        return DownfallHook.AfterModifying(cs, modifiers, m => m.AfterModifyCompiledFunction(result, player));
    }

    public static Task AfterCompilingFunction(PlayerChoiceContext ctx, ICombatState cs, Player player,
        CardPileAddResult result)
    {
        return DownfallHook.Dispatch<IAfterCompilingFunction>(cs, ctx,
            m => m.AfterCompilingFunction(ctx, player, result));
    }
}