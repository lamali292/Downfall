using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;

namespace Downfall.DownfallCode.Abstract;

public class DownfallCardModifier : CardModifier, ICustomModel
{
    public virtual bool ShouldGlowGold => false;
    
    protected LocString Description => new("card_modifiers", Id.Entry + ".description");
}


[HarmonyPatch(typeof(NHandCardHolder), "get_ShouldGlowGold")]
internal static class HandCardHolderGlowGoldPatch
{
    private static void Postfix(NHandCardHolder __instance, ref bool __result)
    {
        if (__result) return;

        var model = __instance.CardNode?.Model;
        if (model == null) return;

        if (CardModifier.Modifiers(model).OfType<DownfallCardModifier>().Any(e => e.ShouldGlowGold))
            __result = true;
    }
}