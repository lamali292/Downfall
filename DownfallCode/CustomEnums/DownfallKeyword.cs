using BaseLib.Patches.Content;
using Downfall.DownfallCode.Extensions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.CustomEnums;

public class DownfallKeyword
{
    [CustomEnum] public static CardKeyword Echo;
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.Title), MethodType.Getter)]
public static class PatchEchoTitle
{
    static void Postfix(CardModel __instance, ref string __result)
    {
        if (!__instance.IsEcho()) return;
        
        var echoLoc = new LocString("card_keywords", "DOWNFALL-ECHO.card_title");
        echoLoc.Add("card", __result);
        __result = echoLoc.GetFormattedText();
    }
}