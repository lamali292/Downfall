using Downfall.Code.Interfaces;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Downfall.Code.Patches;

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
        if (__instance.Model is not IHasSecondAmount power) return;

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
}