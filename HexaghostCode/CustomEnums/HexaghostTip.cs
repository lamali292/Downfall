using BaseLib.Patches.Content;
using HarmonyLib;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;

namespace Hexaghost.HexaghostCode.CustomEnums;

public static class HexaghostTip
{
    [CustomEnum] public static StaticHoverTip Ignite;
    [CustomEnum] public static StaticHoverTip Extinguish;
    [CustomEnum] public static StaticHoverTip Wheel;
}

[HarmonyPatch(typeof(Creature), nameof(Creature.HoverTips), MethodType.Getter)]
public static class PatchCreatureHoverTips
{
    public static void Postfix(Creature __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance.Player == null || !HexaghostCmd.IsGhostwheelActivated(__instance.Player))
            return;
        __result = __result.Concat(
        [
            HoverTipFactory.Static(HexaghostTip.Wheel),
            HoverTipFactory.FromKeyword(HexaghostKeyword.Advance),
            HoverTipFactory.Static(HexaghostTip.Ignite),
            HoverTipFactory.Static(HexaghostTip.Extinguish)
        ]);
    }
}