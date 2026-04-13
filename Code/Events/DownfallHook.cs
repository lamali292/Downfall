using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Core.Champ;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Events;

public static class DownfallHook
{
    private static async Task Dispatch<T>(CombatState combatState, Func<T, Task> action)
        where T : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
            await action(model);
    }

    private static async Task Dispatch<T>(CombatState combatState, PlayerChoiceContext ctx, Func<T, Task> action)
        where T : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
        {
            var abstractModel = (AbstractModel)(object)model;
            ctx.PushModel(abstractModel);
            await action(model);
            ctx.PopModel(abstractModel);
        }
    }

    private static TResult Aggregate<T, TResult>(CombatState combatState, TResult seed,
        Func<T, TResult, TResult> action)
        where T : class => 
        combatState.IterateHookListeners().OfType<T>()
            .Aggregate(seed, (current, model) => action(model, current));
    

    private static bool Any<T>(CombatState combatState, Func<T, bool> predicate)
        where T : class => 
        combatState.IterateHookListeners().OfType<T>().Any(predicate);
    

    public static Task OnDrained(CombatState cs, Player player, int amount) => 
        Dispatch<IOnDrained>(cs, m => m.OnDrained(player, amount));
    

    public static Task OnDrained(CombatState cs, PlayerChoiceContext ctx, Player player, int amount) => 
        Dispatch<IOnDrained>(cs, ctx, m => m.OnDrained(player, amount));
    

    public static Task OnCardChanted(CombatState cs, PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay) => 
        Dispatch<IOnChant>(cs, ctx, m => m.OnCardChanted(card, ctx, cardPlay));
    
    public static Task OnCompile(PlayerChoiceContext ctx, CombatState cs,
        List<AutomatonCardModel> snapshot, FunctionCard functionCard, CardPlay cardPlay) => 
        Dispatch<IOnCompile>(cs, ctx, m => m.OnCompile(ctx, snapshot, functionCard, cardPlay));

    public static Task OnAwaken(CombatState cs, PlayerChoiceContext ctx, Player player) => 
        Dispatch<IOnAwaken>(cs, ctx, m => m.OnAwaken(ctx, player));
    

    public static Task OnCardEncoded(CombatState cs, PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay) => 
        Dispatch<IOnEncode>(cs, ctx, m => m.OnCardEncoded(ctx, card, cardPlay));
    

    public static Task OnFinisher(CombatState cs, PlayerChoiceContext ctx, CardPlay cardPlay) =>
        Dispatch<IOnFinisher>(cs, ctx, m => m.OnFinisher(ctx, cardPlay));
    

    public static Task OnStanceChange(CombatState cs, PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance) => 
        Dispatch<IOnStanceChange>(cs, ctx, m => m.OnStanceChange(ctx, player, oldStance, newStance));
    

    public static int ModifySkillBonus<TPower>(CombatState cs, ChampStanceModel stanceModel, int baseAmount)
        where TPower : PowerModel => 
         Aggregate<IModifySkillBonus, int>(cs, baseAmount,
                (m, current) => m.ModifySkillBonus<TPower>(stanceModel, current));
    
    public static int ModifyCounterStrikeCount(CombatState cs, Player player, int baseAmount) => 
        Aggregate<IModifyCounterStrikeCount, int>(cs, baseAmount, 
            (m, current) => m.ModifyCounterStrikeCount(player, current));
    

    public static bool IgnoreChargeCap(CombatState cs, Player player) =>  
        Any<IIgnoreChampChargeCap>(cs, m => m.IgnoreChargeCap(player));
    
    public static int ModifyFinisherBonus(CombatState cs, ChampStanceModel stanceModel, int baseAmount) =>
        Aggregate<IModifyFinisherBonus, int>(cs, baseAmount,
            (m, current) => m.ModifyFinisherBonus(stanceModel, current));
}