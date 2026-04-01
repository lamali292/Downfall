using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Events;

public static class DownfallHook
{
    
    public static async Task OnDrained(Player player, int amount)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        foreach (var model in combatState.IterateHookListeners().OfType<IOnDrained>())
        {
            await model.OnDrained(player, amount);
        }
    }
    
    public static async Task OnCardChanted(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        var combatState = card.CombatState;
        if (combatState == null) return;
        foreach (var model in combatState.IterateHookListeners().OfType<IOnChant>())
        {
            var abstractModel = (AbstractModel)model;
            ctx.PushModel(abstractModel);
            await model.OnCardChanted(card, ctx, cardPlay);
            ctx.PopModel(abstractModel);
        }
    }
    
    public static async Task OnDrained(PlayerChoiceContext ctx, Player player, int amount)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        foreach (var model in combatState.IterateHookListeners().OfType<IOnDrained>())
        {
            var abstractModel = (AbstractModel)model;
            ctx.PushModel(abstractModel);
            await model.OnDrained(player, amount);
            ctx.PopModel(abstractModel);
        }
    }
    
    public static async Task OnCompile(PlayerChoiceContext ctx, CombatState combatState, 
        List<AutomatonCardModel> snapshot, FunctionCard functionCard, CardPlay cardPlay)
    {
        
        foreach (var model in combatState.IterateHookListeners().OfType<IOnCompile>())
        {
            var abstractModel = (AbstractModel)model;
            ctx.PushModel(abstractModel);
            await model.OnCompile(ctx, snapshot, functionCard, cardPlay);
            ctx.PopModel(abstractModel);
        }
    }
    
    public static async Task OnAwaken(PlayerChoiceContext ctx, Player player)
    {
        var combatState = player.Creature.CombatState;
        if (combatState == null) return;
        foreach (var model in combatState.IterateHookListeners().OfType<IOnAwaken>())
        {
            var abstractModel = (AbstractModel)model;
            ctx.PushModel(abstractModel);
            await model.OnAwaken(ctx, player);
            ctx.PopModel(abstractModel);
        }
    }
    
    public static async Task OnCardEncoded(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        
        var combatState = card.CombatState;
        if (combatState == null) return;
        foreach (var model in combatState.IterateHookListeners().OfType<IOnEncode>())
        {
            var abstractModel = (AbstractModel)model;
            ctx.PushModel(abstractModel);
            await model.OnCardEncoded(ctx, card, cardPlay);
            ctx.PopModel(abstractModel);
        }
    }

    
    
    // add more custom hooks here...
}