using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Patches.KaleidoscopePatch;

[HarmonyPatch(typeof(Kaleidoscope), "<AfterObtained>b__7_0")]
static class KaleidoscopePoolFilter
{
    static void Postfix(Kaleidoscope __instance, CardPoolModel p, ref bool __result)
    {
        if (!__result) return;
        var player = __instance.Owner;
        if (!PoolClassifier.IsDownfallChar(player.Character) && !PoolClassifier.IsUnmoddedChar(player.Character)) return;
        var mode = PrismaticModeConfigSync.For(player.NetId);
        __result = PoolClassifier.Allows(mode, p, player);
    }
}


