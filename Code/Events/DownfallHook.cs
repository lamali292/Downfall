using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.Automaton.Token;
using Downfall.Code.Core.Champ;
using Downfall.Code.Core.Guardian;
using Downfall.Code.Core.Hexaghost;
using Downfall.Code.Ghostflames;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Events;

public static class DownfallHook
{
    private static async Task Dispatch<T>(ICombatState combatState, Func<T, Task> action)
        where T : class
    {
        foreach (var model in combatState.IterateHookListeners().OfType<T>())
            await action(model);
    }

    private static async Task Dispatch<T>(ICombatState combatState, PlayerChoiceContext ctx, Func<T, Task> action)
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

    private static TResult Aggregate<T, TResult>(ICombatState combatState, TResult seed,
        Func<T, TResult, TResult> action)
        where T : class
    {
        return combatState.IterateHookListeners().OfType<T>()
            .Aggregate(seed, (current, model) => action(model, current));
    }


    private static bool Any<T>(ICombatState combatState, Func<T, bool> predicate)
        where T : class
    {
        return combatState.IterateHookListeners().OfType<T>().Any(predicate);
    }

    public static Task OnDrained(ICombatState cs, PlayerChoiceContext ctx, Player player, int amount)
    {
        return Dispatch<IOnDrained>(cs, ctx, m => m.OnDrained(ctx, player, amount));
    }


    public static Task OnCardChanted(ICombatState cs, PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        return Dispatch<IOnChant>(cs, ctx, m => m.OnCardChanted(card, ctx, cardPlay));
    }

    public static Task OnCompile(PlayerChoiceContext ctx, ICombatState cs,
        List<AutomatonCardModel> snapshot, FunctionCard functionCard, CardPlay cardPlay)
    {
        return Dispatch<IOnCompile>(cs, ctx, m => m.OnCompile(ctx, snapshot, functionCard, cardPlay));
    }

    public static Task OnAwaken(ICombatState cs, PlayerChoiceContext ctx, Player player)
    {
        return Dispatch<IOnAwaken>(cs, ctx, m => m.OnAwaken(ctx, player));
    }


    public static Task OnCardEncoded(ICombatState cs, PlayerChoiceContext ctx, CardModel card, CardPlay cardPlay)
    {
        return Dispatch<IOnEncode>(cs, ctx, m => m.OnCardEncoded(ctx, card, cardPlay));
    }


    public static Task OnFinisher(ICombatState cs, PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Dispatch<IOnFinisher>(cs, ctx, m => m.OnFinisher(ctx, cardPlay));
    }


    public static Task OnChampStanceChange(ICombatState cs, PlayerChoiceContext ctx, Player player,
        ChampStanceModel oldStance,
        ChampStanceModel newStance)
    {
        return Dispatch<IOnChampStanceChange>(cs, ctx, m => m.OnChampStanceChange(ctx, player, oldStance, newStance));
    }


    public static Task OnGuardianModeChange(ICombatState cs, PlayerChoiceContext ctx, Player player, GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        return Dispatch<IOnGuardianModeChange>(cs, m => m.OnGuardianModeChange(ctx, player, oldMode, newMode));
    }

    public static int ModifySkillBonus<TPower>(ICombatState cs, ChampStanceModel stanceModel, int baseAmount)
        where TPower : PowerModel
    {
        return Aggregate<IModifySkillBonus, int>(cs, baseAmount,
            (m, current) => m.ModifySkillBonus<TPower>(stanceModel, current));
    }

    public static int ModifyCounterStrikeCount(ICombatState cs, Player player, int baseAmount)
    {
        return Aggregate<IModifyCounterStrikeCount, int>(cs, baseAmount,
            (m, current) => m.ModifyCounterStrikeCount(player, current));
    }

    public static int ModifyCollectorDoomDamage(ICombatState cs, Creature creature, int baseAmount)
    {
        return Aggregate<IModifyCollectorDoomDamage, int>(cs, baseAmount,
            (m, current) => m.ModifyCollectorDoomDamage(creature, current));
    }

    public static bool IgnoreChargeCap(ICombatState cs, Player player)
    {
        return Any<IIgnoreChampChargeCap>(cs, m => m.IgnoreChargeCap(player));
    }

    public static int ModifyFinisherBonus(ICombatState cs, ChampStanceModel stanceModel, int baseAmount)
    {
        return Aggregate<IModifyFinisherBonus, int>(cs, baseAmount,
            (m, current) => m.ModifyFinisherBonus(stanceModel, current));
    }

    public static bool PreventDoomRemoval(ICombatState cs, Creature creature)
    {
        return Any<IPreventDoomRemoval>(cs, m => m.PreventDoomRemoval(creature));
    }

    public static bool PreventCollectedDraw(ICombatState cs, Player player)
    {
        return Any<IPreventCollectedDraw>(cs, m => m.PreventCollectedDraw(player));
    }

    public static Task OnPyre(ICombatState cs, PlayerChoiceContext ctx, CardModel card, CardModel pyred)
    {
        return Dispatch<IOnPyre>(cs, ctx, m => m.OnPyre(ctx, card, pyred));
    }

    public static Task AfterCustomDraw(ICombatState cs, PlayerChoiceContext ctx, Player player, PileType pile,
        CardPileAddResult result)
    {
        return Dispatch<IAfterCustomDraw>(cs, ctx, m => m.AfterCustomDraw(player, pile, result));
    }

    public static Task BeforeCardEntersStasis(ICombatState cs, PlayerChoiceContext ctx, CardModel card,
        AbstractModel source)
    {
        return Dispatch<IBeforeCardEntersStasis>(cs, ctx, m => m.BeforeCardEntersStasis(ctx, card, source));
    }

    public static int ModifyGhostflameEffectAdditive(ICombatState cs, Player owner, GhostflameModel bolsteringGhostflame)
    {
        return Aggregate<IModifyGhostflameEffectAdditive, int>(cs, 0,
            (m, current) => m.ModifyGhostflameEffectAdditive(owner, bolsteringGhostflame));
    }

    public static Task AfterWheelRetract(ICombatState cs, PlayerChoiceContext ctx, Player player, GhostflameModel ghostflame, int ghostflameIndex, bool silent)
    {
        return Dispatch<IWheelMoved>(cs, ctx, m => m.AfterWheelRetract(ctx, player, ghostflame, ghostflameIndex, silent));
    }

    public static Task AfterWheelAdvance(ICombatState cs, PlayerChoiceContext ctx, Player player, GhostflameModel ghostflame, int ghostflameIndex, bool silent)
    {
        return Dispatch<IWheelMoved>(cs, ctx, m => m.AfterWheelAdvance(ctx, player, ghostflame, ghostflameIndex, silent));
    }
}