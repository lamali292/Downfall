using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Patches.KaleidoscopePatch;

[HarmonyPatch(typeof(PrismaticGem), nameof(PrismaticGem.ModifyCardRewardCreationOptions))]
static class PrismaticGemPoolFilter
{
    static void Postfix(PrismaticGem __instance, Player player, ref CardCreationOptions __result)
    {
        if (__instance.Owner != player) return;
        if (!PoolClassifier.IsDownfallChar(player.Character) && !PoolClassifier.IsUnmoddedChar(player.Character)) return;
        var mode = PrismaticModeConfigSync.For(player.NetId);
        if (mode == PrismaticMode.All) return;

        var kept = __result.CardPools.Where(p =>
            p.IsColorless       
            ||  p == player.Character.CardPool
            || PoolClassifier.Allows(mode, p, player)
        ).ToList();

        if (kept.Count == 0) return;                             
        __result = __result.WithCardPools(kept);
    }
}