using Downfall.Code.Core.Hexaghost.Ghostflames;
using Downfall.Code.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

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

    public static async Task Advance(PlayerChoiceContext ctx, Player player, bool silent = false)
    {
        await MoveTo(player, GetNextIndex(player), ctx);
        await DownfallHook.AfterWheelAdvance(player.Creature.CombatState!, ctx, player, GetCurrentFlame(player),
            GetCurrentIndex(player), silent);
    }

    public static async Task Retract(PlayerChoiceContext ctx, Player player, bool silent = false)
    {
        await MoveTo(player, GetPreviousIndex(player), ctx);
        await DownfallHook.AfterWheelRetract(player.Creature.CombatState!, ctx, player, GetCurrentFlame(player),
            GetCurrentIndex(player), silent);
    }

    public static async Task MoveToRandom(PlayerChoiceContext ctx, Player player, bool silent = false)
    {
        var wheel = GetWheel(player);
        var current = GetCurrentIndex(player);
        var rng = player.RunState.Rng.Niche;
        var candidates = Enumerable.Range(0, wheel.Length).Where(i => i != current).ToArray();
        var randomIndex = rng.NextItem(candidates);
        await MoveTo(player, randomIndex, ctx, silent);
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


    private static Task MoveTo(Player player, int index, PlayerChoiceContext? ctx, bool silent = false)
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

    public static async Task Ignite(PlayerChoiceContext ctx, Player player, bool force = false)
    {
        var flame = GetCurrentFlame(player);
        if (flame.IsIgnited && !force) return;
        flame.IsIgnited = true;
        HexaghostVisualsBridge.Refresh(player);
        await flame.OnIgnite(ctx);
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
}