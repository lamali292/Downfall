using HarmonyLib;
using MegaCrit.Sts2.Core.Modding;

namespace DataGen.DataGenCode;

[ModInitializer("Load")]
public class ModEntry
{
    public static void Load()
    {
        
        new Harmony("visible_a_9").PatchAll();
    }
}