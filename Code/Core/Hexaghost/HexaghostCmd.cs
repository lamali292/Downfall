using Downfall.Code.Core.Hexaghost.Ghostflames;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Core.Hexaghost;

public static class HexaghostCmd
{
    public static GhostflameModel[] GetWheel(Player player) => HexaghostModel.Wheel[player];
    public static int GetCurrentIndex(Player player) => HexaghostModel.CurrentIndex[player];
    public static GhostflameModel GetCurrentFlame(Player player) => GetWheel(player)[GetCurrentIndex(player)];
    

    public static T? GetFlameOfType<T>(Player player) where T : GhostflameModel
        => GetWheel(player).OfType<T>().FirstOrDefault();

    public static int GetIgnitedCount(Player player)
        => GetWheel(player).Count(f => f.IsIgnited);

    public static async Task Advance(Player player, PlayerChoiceContext? ctx)
    {
        var newIndex = (GetCurrentIndex(player) + 1) % 6;
        await MoveTo(player, newIndex, ctx);
        //await DownfallHook.OnGhostflameAdvance(player, ctx);
    }

    public static async Task Retract(Player player, PlayerChoiceContext? ctx)
    {
        var newIndex = (GetCurrentIndex(player) + 5) % 6;
        await MoveTo(player, newIndex, ctx);
        //await DownfallHook.OnGhostflameRetract(player, ctx);
    }
    public static async Task MoveTo(Player player, int index, PlayerChoiceContext? ctx)
    {
        HexaghostModel.CurrentIndex[player] = index;
        GetCurrentFlame(player).Extinguish();
        DownfallMainFile.Logger.Info($"Hexaghost moved to {index}");
        HexaghostVisualsBridge.Refresh(player);
    }
    
    public static async Task<bool> IsIgnited(Player player)
    {
        return GetCurrentFlame(player).IsIgnited;
    }

    public static async Task Ignite(Player player)
    {
        var flame = GetCurrentFlame(player);
        if (flame.IsIgnited) return;
        flame.IsIgnited = true;
        HexaghostVisualsBridge.Refresh(player);
        await flame.OnIgnite();
    }

    public static async Task Extinguish(Player player)
    {
        GetCurrentFlame(player).Extinguish();
        HexaghostVisualsBridge.Refresh(player);
    }

    public static async Task ResetWheel(Player player)
    {
        foreach (var flame in GetWheel(player))
            flame.Extinguish();
        HexaghostModel.ResetWheel(player);
        HexaghostVisualsBridge.Refresh(player);
    }
    
}