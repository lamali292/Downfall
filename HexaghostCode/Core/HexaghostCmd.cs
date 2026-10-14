using BaseLib.Utils;
using Downfall.DownfallCode.Commands;
using Godot;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Events;
using Hexaghost.HexaghostCode.History;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace Hexaghost.HexaghostCode.Core;

public static class HexaghostCmd
{
    public static GhostflameModel[] GetWheel(Player player)
    {
        return HexaghostModel.Wheel.Get(player) ?? [];
    }

    /// <summary>
    ///     All Afterlife-keyworded cards available to the player. Hexaghost players draw only from
    ///     their own pool; other characters draw Afterlife cards from every pool.
    /// </summary>
    public static IEnumerable<CardModel> GetAfterlifeCards(Player player, int amount)
    {
        return DownfallCardCmd.GetSpecificCards<Hexaghost>(player, c => c.Keywords.Contains(HexaghostKeyword.Afterlife),
            amount);
    }


 

    public static void ActivateGhostwheel(Player player)
    {
        HexaghostModel.Active.Set(player, true);
    }

    public static bool IsGhostwheelActivated(Player player)
    {
        return HexaghostModel.Active.Get(player);
    }

    public static int GetCurrentIndex(Player player)
    {
        return HexaghostModel.CurrentIndex.Get(player);
    }

    public static GhostflameModel GetCurrentFlame(Player player)
    {
        return GetWheel(player)[GetCurrentIndex(player)];
    }


    public static T? GetFlameOfType<T>(Player player) where T : GhostflameModel
    {
        return GetWheel(player).OfType<T>().FirstOrDefault();
    }

    public static int GetIgnitedCount(Player player)
    {
        return GetWheel(player).Count(f => f.IsIgnited);
    }

    public static bool AllIgnited(Player player)
    {
        return GetWheel(player).All(f => f.IsIgnited);
    }


    private static int GetPreviousIndex(Player player)
    {
        var wheel = GetWheel(player);
        return (GetCurrentIndex(player) + wheel.Length - 1) % wheel.Length;
    }

    private static int GetNextIndex(Player player)
    {
        var wheel = GetWheel(player);
        return (GetCurrentIndex(player) + 1) % wheel.Length;
    }

    public static async Task Advance(PlayerChoiceContext ctx, Player player, AbstractModel? source, bool endOfTurnAdvance = false)
    {
        if (TestMode.IsOff) SfxCmd.Play("event:/sfx/characters/hexaghost-hexaghost/advance");
        await MoveTo(player, GetNextIndex(player));
        var combatState = player.Creature.CombatState!;
        CombatManager.Instance.History.Add(combatState,
            new WheelAdvanceEntry(source, endOfTurnAdvance, player.Creature, combatState.RoundNumber,
                player.Creature.Side, CombatManager.Instance.History, combatState.Players));
        await HexaghostHook.AfterWheelAdvance(player.Creature.CombatState!, ctx, player, source,
            GetCurrentFlame(player),
            GetCurrentIndex(player), endOfTurnAdvance);
    }

    public static async Task Retract(PlayerChoiceContext ctx, Player player, AbstractModel? source)
    {
        if (TestMode.IsOff) SfxCmd.Play("event:/sfx/characters/hexaghost-hexaghost/retract");
        await MoveTo(player, GetPreviousIndex(player));
        var combatState = player.Creature.CombatState!;
        CombatManager.Instance.History.Add(combatState,
            new WheelRetractEntry(source, player.Creature, combatState.RoundNumber,
                player.Creature.Side, CombatManager.Instance.History, combatState.Players));
        await HexaghostHook.AfterWheelRetract(player.Creature.CombatState!, ctx, player, source,
            GetCurrentFlame(player),
            GetCurrentIndex(player));
    }

    /// <summary>How many Advances/Retracts the player performed this turn (the end-of-turn advance excluded).</summary>
    public static int CountWheelMovesThisTurn(Creature creature)
    {
        var combatState = creature.CombatState;
        return CombatManager.Instance.History.Entries.OfType<WheelMoveEntry>()
            .Count(e => e is not WheelAdvanceEntry { IsEndOfTurnAdvance: true } && e.Actor == creature && e.HappenedThisTurn(combatState));
    }
    

    public static Task ReplaceCurrentWithRandom(Player player)
    {
        var wheel = GetWheel(player);
        var currentIdx = GetCurrentIndex(player);
        var rng = player.RunState.Rng.Niche;

        var current = wheel[currentIdx];
        var isOffclass = current.IsOffclass;
        var currentType = current.GetType();
        var candidates = HexaghostModelDb.AllGhostflames
            .Where(f => f.GetType() != currentType && f.IsOffclass == isOffclass).ToArray();
        var randomFlame = rng.NextItem(candidates);

        if (randomFlame == null) return Task.CompletedTask;
        wheel[currentIdx] = randomFlame.ToMutable(player);
        Refresh(player);
        return Task.CompletedTask;
    }


    private static Task MoveTo(Player player, int index)
    {
        if (player.PlayerCombatState == null) return Task.CompletedTask;
        ActivateGhostwheel(player);
        HexaghostModel.CurrentIndex[player] = index;
        var flame = GetCurrentFlame(player);
        flame.Extinguish();
        flame.UpdateVisuals();
        Refresh(player);
        return Task.CompletedTask;
    }

    public static bool IsIgnited(Player player)
    {
        return GetCurrentFlame(player).IsIgnited;
    }

    public static bool IsPreviousIgnited(Player player)
    {
        return GetWheel(player)[GetPreviousIndex(player)].IsIgnited;
    }

    public static bool IsNextIgnited(Player player)
    {
        return GetWheel(player)[GetNextIndex(player)].IsIgnited;
    }

    public static Task IgnitePrevious(PlayerChoiceContext ctx, Player player)
    {
        return IgniteAt(ctx, player, GetPreviousIndex(player));
    }

    public static Task IgniteNext(PlayerChoiceContext ctx, Player player)
    {
        return IgniteAt(ctx, player, GetNextIndex(player));
    }

    public static Task Ignite(PlayerChoiceContext ctx, Player player)
    {
        return IgniteAt(ctx, player, GetCurrentIndex(player));
    }

    public static async Task IgniteAt(PlayerChoiceContext ctx, Player player, int index)
    {
        ActivateGhostwheel(player);
        await Cmd.Wait(0.05f);
        var flame = GetWheel(player)[index];
        if (!flame.IsIgnited)
            flame.IsIgnited = true;

        var allIgnited = AllIgnited(player);
        flame.SetIgniteProgress();
        Refresh(player);
        await flame.OnIgnite(ctx);
        await HexaghostHook.AfterGhostwheelIgnited(player.Creature.CombatState!, ctx, player, flame, index);
        await Cmd.Wait(0.05f);
        if (allIgnited)
            await HexaghostHook.AfterGhostwheelAllIgnited(player.Creature.CombatState!, ctx, player, flame, index);
    }


    public static async Task IgniteAll(PlayerChoiceContext ctx, Player player)
    {
        var wheel = GetWheel(player);
        for (var i = 0; i < wheel.Length; i++) await IgniteAt(ctx, player, i);
    }

    public static Task ExtinguishAllExceptThis(PlayerChoiceContext ctx, Player player, GhostflameModel model)
    {
        foreach (var f in GetWheel(player).Where(e => e != model))
            f.Extinguish();
        Refresh(player);
        return Task.CompletedTask;
    }


    public static Task Extinguish(Player player, bool silent = false)
    {
        GetCurrentFlame(player).Extinguish();
        if (silent) return Task.CompletedTask;
        Refresh(player);
        return Task.CompletedTask;
    }

    public static void Refresh(Player player)
    {
        if (!IsGhostwheelActivated(player)) return;
        HexaghostVisualsBridge.Refresh(player);
    }

    public static Task<int> ResetWheel(Player player)
    {
        Cmd.Wait(0.1f);
        var a = GetWheel(player).Count(flame => flame.Extinguish());
        Cmd.Wait(0.1f);
        HexaghostModel.ResetWheel(player);
        Cmd.Wait(0.1f);
        Refresh(player);
        return Task.FromResult(a);
    }

    public static void SetCurrentGhostflame(Player player, GhostflameModel ghostflame)
    {
        ghostflame.AssertCanonical();
        ActivateGhostwheel(player);
        GetWheel(player)[GetCurrentIndex(player)] = ghostflame.ToMutable(player);
        Refresh(player);
    }

    public static AttackCommand AfterlifeAttack(CardModel card, CardPlay? cardPlay)
    {
        AttackCommand a;
        if (card.DynamicVars.ContainsKey("CalculatedDamage"))
            a = DamageCmd.Attack(card.DynamicVars.CalculatedDamage);
        else if (card.DynamicVars.ContainsKey("Damage"))
            a = DamageCmd.Attack(card.DynamicVars.Damage.BaseValue);
        else
            throw new Exception(
                $"Card {card.Title} does not have a damage variable supported by CommonActions.CardAttack");
        a = a.FromCardCompatibility(card, cardPlay);
        if (cardPlay?.Target != null) return a.Targeting(cardPlay.Target);
        if (card.CombatState != null)
            return card.TargetType == TargetType.AllEnemies
                ? a.TargetingAllOpponents(card.CombatState)
                : a.TargetingRandomOpponents(card.CombatState);
        throw new InvalidOperationException("Afterlife attack failed!");
    }
}