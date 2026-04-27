using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Events;

public static class AwakenedHook
{
   
    public static Task OnDrained(ICombatState cs, PlayerChoiceContext ctx, Player player, int amount)
    {
        return DownfallHook.Dispatch<IOnDrained>(cs, ctx, m => m.OnDrained(ctx, player, amount));
    }


    public static Task OnCardChanted(ICombatState cs, PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        return DownfallHook.Dispatch<IOnChant>(cs, ctx, m => m.OnCardChanted(card, ctx, cardPlay));
    }
    
    public static Task OnAwaken(ICombatState cs, PlayerChoiceContext ctx, Player player)
    {
        return DownfallHook.Dispatch<IOnAwaken>(cs, ctx, m => m.OnAwaken(ctx, player));
    }


 
}