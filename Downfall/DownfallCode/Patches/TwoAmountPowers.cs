using System.Runtime.CompilerServices;
using Downfall.DownfallCode.Interfaces;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.DownfallCode.Patches;

/**
 * Credits to kiooeht, this displays a second amount for powers that require tracking multiple values.
 * https://github.com/erasels/Minty-Spire-2/blob/main/src/combat/TwoAmountPowers.cs
 */
[HarmonyPatch(typeof(NPower))]
internal static class TwoAmountPowers
{
    [HarmonyPatch("RefreshAmount")]
    [HarmonyPostfix]
    private static void ShowSecondAmount(NPower __instance)
    {
        if (!__instance.IsNodeReady()) return;
        if (__instance._model is not IHasSecondAmount power) return;

        if (!__instance.HasNode("Amount2Label"))
        {
            var label = __instance.GetNode<MegaLabel>("%AmountLabel");
            var newLabel = (MegaLabel)label.Duplicate();
            newLabel.Name = "Amount2Label";
            newLabel.UniqueNameInOwner = false;
            newLabel.Visible = false;
            __instance.AddChild(newLabel);
            __instance.MoveChild(newLabel, label.GetIndex());
        }

        var amount1Label = __instance.GetNode<MegaLabel>("%AmountLabel");
        var amount2Label = __instance.GetNode<MegaLabel>("Amount2Label");
        var text = power.GetSecondAmount();

        if (string.IsNullOrEmpty(text))
        {
            amount2Label.Visible = false;
            return;
        }

        amount2Label.Visible = true;
        amount2Label.SetTextAutoSize(text);
        var fontSize = amount2Label.GetThemeFontSize(ThemeConstants.Label.FontSize);
        amount2Label.Position = amount1Label.Position + new Vector2(0, -(fontSize + 2));
    }
    [HarmonyPatch(nameof(NPower.SubscribeToModelEvents))]
    [HarmonyPostfix]
    private static void Subscribe(NPower __instance)
    {
        if (__instance._model is IHasSecondAmount power)
            SecondAmountRegistry.Register(power, __instance.RefreshAmount);
    }

    [HarmonyPatch(nameof(NPower.UnsubscribeFromModelEvents))]
    [HarmonyPostfix]
    private static void Unsubscribe(NPower __instance)
    {
        if (__instance._model is IHasSecondAmount power)
            SecondAmountRegistry.Unregister(power);
    }
}

internal static class SecondAmountRegistry
{
    internal static readonly ConditionalWeakTable<IHasSecondAmount, Action> RefreshActions = new();

    internal static void Register(IHasSecondAmount power, Action refresh)
        => RefreshActions.AddOrUpdate(power, refresh);

    internal static void Unregister(IHasSecondAmount power)
        => RefreshActions.Remove(power);
}


