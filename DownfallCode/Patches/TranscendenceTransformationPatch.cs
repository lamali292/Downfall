using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Downfall.DownfallCode.Patches;

public static class TranscendenceHooks
{
    // (starterCard, resultCard)
    public static event Action<CardModel, CardModel>? OnTransformed;

    internal static void RaiseTransformed(CardModel starter, CardModel result)
    {
        if (OnTransformed == null) return;
        foreach (var d in OnTransformed.GetInvocationList())
        {
            var handler = (Action<CardModel, CardModel>)d;
            try { handler(starter, result); }
            catch (Exception e) { DownfallMainFile.Logger.Error($"Transcendence handler failed: {e}"); }
        }
    }
}

[HarmonyPatch(typeof(ArchaicTooth), nameof(ArchaicTooth.GetTranscendenceTransformedCard))]
internal static class TranscendenceTransformationPatch
{
    [HarmonyPostfix]
    private static void Postfix(CardModel starterCard, CardModel __result)
        => TranscendenceHooks.RaiseTransformed(starterCard, __result);
}