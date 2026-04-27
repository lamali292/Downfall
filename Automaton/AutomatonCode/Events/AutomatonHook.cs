using Automaton.AutomatonCode.Cards;
using Automaton.AutomatonCode.Cards.Token;
using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Events;

public static class AutomatonHook
{
 

    public static Task OnCompile(PlayerChoiceContext ctx, ICombatState cs,
        List<AutomatonCardModel> snapshot, FunctionCard functionCard, CardPlay cardPlay)
    {
        return DownfallHook.Dispatch<IOnCompile>(cs, ctx, m => m.OnCompile(ctx, snapshot, functionCard, cardPlay));
    }
    

    public static Task OnCardEncoded(ICombatState cs, PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        return DownfallHook.Dispatch<IOnEncode>(cs, ctx, m => m.OnCardEncoded(ctx, card, cardPlay));
    }
    
 
}