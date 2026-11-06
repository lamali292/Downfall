using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.HoverTips;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Patches;

[HarmonyPatch(typeof(Creature), nameof(Creature.HoverTips), MethodType.Getter)]
internal static class SlimeHoverTipPatch
{
    private static void Postfix(Creature __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance.Monster is not SlimeModel slime) return;
        IEnumerable<IHoverTip> tips = [slime.SlimeTip];
        tips = tips.Concat(slime.ExtraTips);
        tips = tips.Concat(__result);
        __result = tips;
    }
}