using HarmonyLib;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Patches;

[HarmonyPatch(typeof(CardModel), nameof(CardModel.OnPlayWrapper))]
internal static class DeadOnPatch
{
    internal static bool LastWasDeadOn;
    internal static CardModel? LastPlayed;

    private static void Prefix(CardModel __instance)
    {
        LastPlayed = __instance;
        LastWasDeadOn = HermitCmd.IsDeadOnInCurrentHandState(__instance);
    }
}