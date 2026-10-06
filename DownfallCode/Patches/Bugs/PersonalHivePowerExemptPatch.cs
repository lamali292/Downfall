using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.DownfallCode.Utils;

// Public: called from standalone submods' own MainFile (e.g. SlimeBoss, a separate assembly
// since the standalone-submods effort), not just code compiled into Downfall.dll itself.
public static class HivePowerExemptRegistry
{
    private static readonly HashSet<Type> Exempt = new();

    public static void Register<T>() where T : MonsterModel => Exempt.Add(typeof(T));
    public static void Register(Type modelType) => Exempt.Add(modelType);

    public static bool IsExempt(MonsterModel monster) =>
        Exempt.Any(t => t.IsInstanceOfType(monster));
}



[HarmonyPatch(typeof(PersonalHivePower), nameof(PersonalHivePower.AfterDamageReceived))]
internal static class PersonalHivePowerExemptPatch
{
    private static bool Prefix(Creature? dealer, ref Task __result)
    {
        if (dealer?.Monster is null || !HivePowerExemptRegistry.IsExempt(dealer.Monster))
            return true;

        __result = Task.CompletedTask;
        return false;
    }
}
