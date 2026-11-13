using Downfall.DownfallCode.Interfaces;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

/// <summary>
/// Adds <see cref="IRelicAllowedForPlayer"/> to the filter of every relic pull. All the other
/// <c>RelicFactory.PullNextRelicFrom*</c> overloads funnel into these two. Pulls run on every client from
/// synchronized state (the deck), so the filter picks the same relic everywhere.
/// </summary>
internal static class RelicPullPlayerFilterPatch
{
    internal static Func<RelicModel, bool> WithPlayerFilter(Player player, Func<RelicModel, bool> filter)
    {
        return relic => filter(relic) && (relic is not IRelicAllowedForPlayer gated || gated.IsAllowedForPlayer(player));
    }
}

[HarmonyPatch(typeof(RelicFactory), nameof(RelicFactory.PullNextRelicFromFront),
    typeof(Player), typeof(RelicRarity), typeof(Func<RelicModel, bool>))]
internal static class RelicPullFromFrontPlayerFilterPatch
{
    [HarmonyPrefix]
    private static void Prefix(Player player, ref Func<RelicModel, bool> filter)
    {
        filter = RelicPullPlayerFilterPatch.WithPlayerFilter(player, filter);
    }
}

[HarmonyPatch(typeof(RelicFactory), nameof(RelicFactory.PullNextRelicFromBack),
    typeof(Player), typeof(RelicRarity), typeof(Func<RelicModel, bool>))]
internal static class RelicPullFromBackPlayerFilterPatch
{
    [HarmonyPrefix]
    private static void Prefix(Player player, ref Func<RelicModel, bool> filter)
    {
        filter = RelicPullPlayerFilterPatch.WithPlayerFilter(player, filter);
    }
}
