using Downfall.Code.Core.Hexaghost.Ghostflames;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Core.Hexaghost;

public static class HexaghostCmd
{
    public static GhostflameModel[] GetWheel(Player player) => HexaghostModel.Wheel[player] ?? [];
    public static int GetCurrentIndex(Player player) => HexaghostModel.CurrentIndex[player];
    public static GhostflameModel GetCurrentFlame(Player player) => GetWheel(player)[GetCurrentIndex(player)];
    

    public static T? GetFlameOfType<T>(Player player) where T : GhostflameModel
        => GetWheel(player).OfType<T>().FirstOrDefault();

    public static int GetIgnitedCount(Player player)
        => GetWheel(player).Count(f => f.IsIgnited);

    
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
    
    public static async Task Advance(PlayerChoiceContext ctx, Player player)
    {
        await MoveTo(player, GetNextIndex(player), ctx);
    }
    
    public static async Task Retract(PlayerChoiceContext ctx, Player player)
    {
        await MoveTo(player, GetPreviousIndex(player), ctx);
    }

    
    public static Task MoveTo(Player player, int index, PlayerChoiceContext? ctx)
    {
        HexaghostModel.CurrentIndex[player] = index;
        GetCurrentFlame(player).Extinguish();
        DownfallMainFile.Logger.Info($"Hexaghost moved to {index}");
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

    public static async Task Ignite(PlayerChoiceContext ctx, Player player)
    {
        var flame = GetCurrentFlame(player);
        if (!flame.IsIgnited)
        {
            flame.IsIgnited = true;
            HexaghostVisualsBridge.Refresh(player);
        }
        await flame.OnIgnite(ctx);
    }

    public static Task Extinguish(Player player)
    {
        GetCurrentFlame(player).Extinguish();
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