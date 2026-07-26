using System.Runtime.CompilerServices;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;

[HarmonyPatch(typeof (NPower))]
public static class InvokeSilentDisplayAmountChangedPatch
{

    [HarmonyPatch("SubscribeToModelEvents")]
    [HarmonyPostfix]
    public static void Subscribe(NPower __instance)
    {
        if (__instance._model is not { } model)
            return;
        UpdateAmountRegistry.Register(model, __instance.RefreshAmount);
    }

    [HarmonyPatch("UnsubscribeFromModelEvents")]
    [HarmonyPostfix]
    public static void Unsubscribe(NPower __instance)
    {
        if (__instance._model is not { } model)
            return;
        UpdateAmountRegistry.Unregister(model);
    }
}

public static class UpdateAmountRegistry
{
    public static readonly ConditionalWeakTable<PowerModel, Action> RefreshActions = new();

    public static void Register(PowerModel power, Action refresh)
    {
        RefreshActions.AddOrUpdate(power, refresh);
    }

    public static void Unregister(PowerModel power)
    {
        RefreshActions.Remove(power);
    }
}

