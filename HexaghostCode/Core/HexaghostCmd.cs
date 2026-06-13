using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.Core;

public static class HexaghostCmd
{
    public static GhostflameModel[] GetWheel(Player player)
    {
        return HexaghostModel.Wheel[player] ?? [];
    }

    public static int GetCurrentIndex(Player player)
    {
        return HexaghostModel.CurrentIndex[player];
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

    public static async Task Advance(PlayerChoiceContext ctx, Player player, AbstractModel? source, bool silent = false,
        bool autoAdvance = false)
    {
        await MoveTo(player, GetNextIndex(player));
        if (!autoAdvance)
            await HexaghostHook.AfterWheelAdvance(player.Creature.CombatState!, ctx, player, source,
                GetCurrentFlame(player),
                GetCurrentIndex(player), silent);
    }

    public static async Task Retract(PlayerChoiceContext ctx, Player player, AbstractModel? source, bool silent = false)
    {
        await MoveTo(player, GetPreviousIndex(player));
        await HexaghostHook.AfterWheelRetract(player.Creature.CombatState!, ctx, player, source,
            GetCurrentFlame(player),
            GetCurrentIndex(player), silent);
    }

    public static async Task MoveToRandom(PlayerChoiceContext ctx, Player player, bool silent = false)
    {
        var wheel = GetWheel(player);
        var current = GetCurrentIndex(player);
        var rng = player.RunState.Rng.Niche;
        var candidates = Enumerable.Range(0, wheel.Length).Where(i => i != current).ToArray();
        var randomIndex = rng.NextItem(candidates);
        await MoveTo(player, randomIndex, silent);
    }

    public static Task ReplaceCurrentWithRandom(Player player)
    {
        var wheel = GetWheel(player);
        var current = GetCurrentIndex(player);
        var rng = player.RunState.Rng.Niche;

        var currentType = wheel[current].GetType();
        var candidates = HexaghostModelDb.AllGhostflames.Where(f => f.GetType() != currentType).ToArray();
        var randomFlame = rng.NextItem(candidates);

        if (randomFlame == null) return Task.CompletedTask;
        wheel[current] = randomFlame.ToMutable(player);
        HexaghostVisualsBridge.Refresh(player);
        return Task.CompletedTask;
    }


    private static Task MoveTo(Player player, int index, bool silent = false)
    {
        HexaghostModel.CurrentIndex[player] = index;
        var flame = GetCurrentFlame(player);
        flame.Extinguish();
        flame.UpdateVisuals();
        if (silent) return Task.CompletedTask;
        HexaghostVisualsBridge.Refresh(player);
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
        await Cmd.Wait(0.05f);
        var flame = GetWheel(player)[index];
        if (!flame.IsIgnited)
            flame.IsIgnited = true;

        flame.SetIgniteProgress();
        HexaghostVisualsBridge.Refresh(player);
        await flame.OnIgnite(ctx);
        await HexaghostHook.AfterGhostwheelIgnited(player.Creature.CombatState!, ctx, player, flame, index);
        await Cmd.Wait(0.05f);
        //todo this should be in the inferno ghostflame, I made this a separate command for now
        if (AllIgnited(player))
        {
            await HexaghostHook.AfterGhostwheelAllIgnited(player.Creature.CombatState!, ctx, player, flame, index);
            /*foreach (var f in GetWheel(player).Where(f => !f.IsActive))
                f.Extinguish();
            HexaghostVisualsBridge.Refresh(player);*/
        }
    
    }


    public static async Task IgniteAll(PlayerChoiceContext ctx, Player player)
    {
        var wheel = GetWheel(player);
        for (var i = 0; i < wheel.Length; i++) await IgniteAt(ctx, player, i);
    }

    public static async Task ExtinguishAllExceptCurrent(PlayerChoiceContext ctx, Player player) {
         foreach (var f in GetWheel(player).Where(f => !f.IsActive))
           f.Extinguish();
         HexaghostVisualsBridge.Refresh(player);
    }
    
    public static Task Extinguish(Player player, bool silent = false)
    {
        GetCurrentFlame(player).Extinguish();
        if (silent) return Task.CompletedTask;
        HexaghostVisualsBridge.Refresh(player);
        return Task.CompletedTask;
    }

    public static Task<int> ResetWheel(Player player)
    {
        var a = GetWheel(player).Count(flame => flame.Extinguish());
        HexaghostModel.ResetWheel(player);
        HexaghostVisualsBridge.Refresh(player);
        return Task.FromResult(a);
    }

    public static void SetCurrentGhostflame(Player player, GhostflameModel ghostflame)
    {
        ghostflame.AssertCanonical();
        GetWheel(player)[GetCurrentIndex(player)] = ghostflame.ToMutable(player);
        HexaghostVisualsBridge.Refresh(player);
    }
}