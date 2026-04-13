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
        where T : class
    {
        return combatState.IterateHookListeners().OfType<T>()
            .Aggregate(seed, (current, model) => action(model, current));
    }

    private static bool Any<T>(CombatState combatState, Func<T, bool> predicate)
        where T : class
    {
        return combatState.IterateHookListeners().OfType<T>().Any(predicate);
    }

    public static async Task OnDrained(Player player, int amount)
    {
        var cs = player.Creature.CombatState;
        if (cs == null) return;
        await Dispatch<IOnDrained>(cs, m => m.OnDrained(player, amount));
    }

    public static async Task OnDrained(PlayerChoiceContext ctx, Player player, int amount)
    {
        var cs = player.Creature.CombatState;
        if (cs == null) return;
        await Dispatch<IOnDrained>(cs, ctx, m => m.OnDrained(player, amount));
    }

    public static async Task OnCardChanted(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        var cs = card.CombatState;
        if (cs == null) return;
        await Dispatch<IOnChant>(cs, ctx, m => m.OnCardChanted(card, ctx, cardPlay));
    }


    public static async Task OnCompile(PlayerChoiceContext ctx, CombatState cs,
        List<AutomatonCardModel> snapshot, FunctionCard functionCard, CardPlay cardPlay)
    {
        await Dispatch<IOnCompile>(cs, ctx, m => m.OnCompile(ctx, snapshot, functionCard, cardPlay));
    }

    public static async Task OnAwaken(PlayerChoiceContext ctx, Player player)
    {
        var cs = player.Creature.CombatState;
        if (cs == null) return;
        await Dispatch<IOnAwaken>(cs, ctx, m => m.OnAwaken(ctx, player));
    }

    public static async Task OnCardEncoded(PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        var cs = card.CombatState;
        if (cs == null) return;
        await Dispatch<IOnEncode>(cs, ctx, m => m.OnCardEncoded(ctx, card, cardPlay));
    }

    public static async Task OnFinisher(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cs = cardPlay.Card.CombatState;
        if (cs == null) return;
        await Dispatch<IOnFinisher>(cs, ctx, m => m.OnFinisher(ctx, cardPlay));
    }

    public static async Task OnStanceChange(PlayerChoiceContext ctx, Player player, ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        var cs = player.Creature.CombatState;
        if (cs == null) return;


        await Dispatch<IOnStanceChange>(cs, ctx, m => m.OnStanceChange(ctx, player, oldStance, newStance));
    }

    public static int ModifySkillBonus<TPower>(ChampStanceModel stanceModel, int baseAmount)
        where TPower : PowerModel
    {
        var cs = stanceModel.Owner.Creature.CombatState;
        return cs == null
            ? baseAmount
            : Aggregate<IModifySkillBonus, int>(cs, baseAmount,
                (m, current) => m.ModifySkillBonus<TPower>(stanceModel, current));
    }

    public static int ModifyCounterStrikeCount(Player player, int baseAmount)
    {
        var cs = player.Creature.CombatState;
        return cs == null
            ? baseAmount
            : Aggregate<IModifyCounterStrikeCount, int>(cs, baseAmount,
                (m, current) => m.ModifyCounterStrikeCount(player, current));
    }

    public static bool IgnoreChargeCap(Player player)
    {
        var cs = player.Creature.CombatState;
        return cs != null && Any<IIgnoreChampChargeCap>(cs, m => m.IgnoreChargeCap(player));
    }
}