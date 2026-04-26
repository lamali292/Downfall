using Downfall.Code.Core.Hexaghost.Ghostflames;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Core.Hexaghost;

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

    public static async Task Advance(PlayerChoiceContext ctx, Player player, CardModel? source, bool silent = false)
    {
        await MoveTo(player, GetNextIndex(player));
        await DownfallHook.AfterWheelAdvance(player.Creature.CombatState!, ctx, player, source, GetCurrentFlame(player),
            GetCurrentIndex(player), silent);
    }

    public static async Task Retract(PlayerChoiceContext ctx, Player player, CardModel? source, bool silent = false)
    {
        await MoveTo(player, GetPreviousIndex(player));
        await DownfallHook.AfterWheelRetract(player.Creature.CombatState!, ctx, player, source, GetCurrentFlame(player),
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
        var candidates = DownfallModelDb.AllGhostflames.Where(f => f.GetType() != currentType).ToArray();
        var randomFlame = rng.NextItem(candidates);

        if (randomFlame == null) return Task.CompletedTask;
        wheel[current] = randomFlame.ToMutable(player);
        HexaghostVisualsBridge.Refresh(player);
        return Task.CompletedTask;
    }


    private static Task MoveTo(Player player, int index, bool silent = false)
    {
        HexaghostModel.CurrentIndex[player] = index;
        GetCurrentFlame(player).Extinguish();
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
        => IgniteAt(ctx, player, GetPreviousIndex(player));

    public static Task IgniteNext(PlayerChoiceContext ctx, Player player)
        => IgniteAt(ctx, player, GetNextIndex(player));
    
    public static Task Ignite(PlayerChoiceContext ctx, Player player)
        => IgniteAt(ctx, player, GetCurrentIndex(player));
    
    public static async Task IgniteAt(PlayerChoiceContext ctx, Player player, int index)
    {
        await Cmd.Wait(0.05f);
        var flame = GetWheel(player)[index];
        if (!flame.IsIgnited)
            flame.IsIgnited = true;
        HexaghostVisualsBridge.Refresh(player);
        await flame.OnIgnite(ctx); ;
        await DownfallHook.AfterGhostwheelIgnited(player.Creature.CombatState!, ctx, player, flame, index);
        await Cmd.Wait(0.05f);
        if (AllIgnited(player))
        {
            await DownfallHook.AfterGhostwheelAllIgnited(player.Creature.CombatState!, ctx, player, flame, index);
            foreach (var f in GetWheel(player))
                f.Extinguish();
            HexaghostVisualsBridge.Refresh(player);
        }
    }
    
    
 
    
    public static async Task IgniteAll(PlayerChoiceContext ctx, Player player)
    {
        var wheel = GetWheel(player);
        var start = GetCurrentIndex(player);
        for (var i = 0; i < wheel.Length; i++)
        {
            await IgniteAt(ctx, player, (start + i) % wheel.Length);
        }
            
    }
    
    public static Task Extinguish(Player player, bool silent = false)
    {
        GetCurrentFlame(player).Extinguish();
        if (silent) return Task.CompletedTask;
        HexaghostVisualsBridge.Refresh(player);
        return Task.CompletedTask;
    }

    public static Task ResetWheel(Player player)
    {
        foreach (var flame in GetWheel(player))
            flame.Extinguish();
        HexaghostModel.ResetWheel(player);
        HexaghostVisualsBridge.Refresh(player);
        return Task.CompletedTask;
    }

    public static void SetCurrentGhostflame(Player player, GhostflameModel ghostflame)
    {
        ghostflame.AssertCanonical();
        GetWheel(player)[GetCurrentIndex(player)] = ghostflame.ToMutable(player);
        HexaghostVisualsBridge.Refresh(player);
    }
}