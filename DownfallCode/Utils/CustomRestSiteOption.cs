using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;

namespace Downfall.DownfallCode.Utils;

public abstract class CustomRestSiteOption(Player owner) : RestSiteOption(owner)
{
    public virtual string? CustomIconPath => null;
}

[HarmonyPatch(typeof(RestSiteOption), "IconPath", MethodType.Getter)]
internal class CustomOrbIconPath
{
    [HarmonyPrefix]
    private static bool Custom(RestSiteOption __instance, ref string __result)
    {
        if (__instance is not CustomRestSiteOption { CustomIconPath: { } path })
            return true;
        __result = path;
        return false;
    }
}