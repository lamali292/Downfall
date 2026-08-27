using Downfall.DownfallCode.Utils;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof(ScrollBoxes), nameof(ScrollBoxes.GenerateRandomBundles))]
public static class ScrollBoxesCustomBundlePatch
{
    private static void Postfix(Player player, ref List<IReadOnlyList<CardModel>> __result)
    {
        if (CustomBundleRegistry.Packages.Count == 0)
            return;

        var rng = player.PlayerRng.Rewards;
        for (var slot = 0; slot < __result.Count; slot++)
            foreach (var pkg in from pkg in CustomBundleRegistry.Packages
                     where pkg.MatchesCharacter(player.Character)
                     where rng.NextInt(100) < pkg.ChancePercent
                     select pkg)
            {
                __result[slot] = pkg.BuildCards();
                break;
            }
    }
}