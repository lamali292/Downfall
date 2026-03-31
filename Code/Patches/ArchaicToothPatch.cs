using Downfall.Code.Cards.Awakened.Ancient;
using Downfall.Code.Cards.Awakened.Basic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Downfall.Code.Patches;

[HarmonyPatch(typeof(ArchaicTooth), "TranscendenceUpgrades", MethodType.Getter)]
public static class ArchaicToothPatch
{
    [HarmonyPostfix]
    private static void AddWatcherTranscendence(ref Dictionary<ModelId, CardModel> __result)
    {
        __result[ModelDb.Card<TalonRake>().Id] = ModelDb.Card<TalonRend>();
    }
}