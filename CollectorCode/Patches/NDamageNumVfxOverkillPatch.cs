using Collector.CollectorCode.Core;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.TestSupport;

namespace Collector.CollectorCode.Patches;

[HarmonyPatch(typeof(NDamageNumVfx))]
public class NDamageNumVfxOverkillPatch
{
    [HarmonyPatch(nameof(NDamageNumVfx.Create), typeof(Creature), typeof(DamageResult))]
    [HarmonyPrefix]
    static bool CreatePrefix(Creature target, DamageResult result, ref NDamageNumVfx? __result)
    {
        if (target.Monster is not TorchheadMonsterModel)
            return true;
        if (TestMode.IsOn)
        {
            __result = null;
            return false;
        }
        __result = NDamageNumVfx.Create(target, result.UnblockedDamage);
        return false;
    }
}