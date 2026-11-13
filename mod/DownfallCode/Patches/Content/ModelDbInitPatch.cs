using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

// Lib mod
public static class PostInitRegistry
{
    private static readonly List<Action> actions = new();

    public static void Register(Action action)
    {
        actions.Add(action);
    }

    internal static void RunAll()
    {
        foreach (var action in actions)
            try
            {
                action();
            }
            catch (Exception e)
            {
                DownfallMainFile.Logger.Error($"Post-init action failed: {e}");
            }
    }
}

[HarmonyPatch(typeof(ModelDb), "InitIds")]
internal static class ModelDbInitPatch
{
    [HarmonyPostfix]
    private static void Postfix()
    {
        PostInitRegistry.RunAll();
    }
}