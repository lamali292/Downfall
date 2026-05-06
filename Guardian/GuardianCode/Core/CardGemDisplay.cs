using BaseLib.Utils;
using Godot;
using Guardian.GuardianCode.Cards;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Guardian.GuardianCode.Core;

public partial class CardGemDisplay : Control
{
    public static void Update(CardModel card)
    {
        var ncard = NCard.FindOnTable(card);
        if (ncard != null)
            UpdateFromNCard(ncard);
    }

    public static void UpdateFromNCard(NCard ncard)
    {
        var display = ncard.GetNodeOrNull<CardGemDisplay>("CardGemDisplay");
        UpdateVisuals(ncard, display);
    }


    private static CardGemDisplay UpdateVisuals(NCard card)
    {
        return UpdateVisuals(card, null);
    }

    private static CardGemDisplay UpdateVisuals(NCard card, CardGemDisplay? display)
    {
        if (card.Model is not (GuardianCardModel guardianCard)
            || guardianCard.GemSlots == 0)
        {
            if (display != null) display.Visible = false;
            return display ?? new CardGemDisplay
            {
                Visible = false,
                MouseFilter = MouseFilterEnum.Ignore
            };
        }
    
        var vBox = display?.GetNodeOrNull<VBoxContainer>("GemSlots");
        if (display == null || vBox == null)
        {
            display ??= new CardGemDisplay()
            {
                MouseFilter = MouseFilterEnum.Ignore,
            };
            vBox = new VBoxContainer
            {
                Name = "GemSlots",
                MouseFilter = MouseFilterEnum.Ignore,
                Position = new Vector2(90, -130)
            };
            display.AddChild(vBox);
        }
    
        display.Visible = true;
    
        var currentGems = guardianCard.Gems;
    
        foreach (var child in vBox.GetChildren())
        {
            vBox.RemoveChild(child);
            child.QueueFree();
        }
    
        for (var i = 0; i < guardianCard.GemSlots; i++)
        {
            var isFilled = i < currentGems.Count;
            var texture = isFilled ? currentGems[i].Icon : GemModel.EmptyIcon;
    
            vBox.AddChild(new TextureRect
            {
                Name = $"Slot_{i}",
                Texture = texture,
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                CustomMinimumSize = new Vector2(60f, 60f),
                MouseFilter = MouseFilterEnum.Ignore
            });
        }
    
        return display;
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard._Ready))]
public static class NCardReadyPatch
{
    [HarmonyPostfix]
    public static void Postfix(NCard __instance)
    {
        if (__instance.GetNodeOrNull<CardGemDisplay>("CardGemDisplay") != null) return;
        var display = new CardGemDisplay { Name = "CardGemDisplay", MouseFilter = Control.MouseFilterEnum.Ignore };
        __instance.AddChildSafely(display);
        CardGemDisplay.UpdateFromNCard(__instance);
    }
}

[HarmonyPatch(typeof(NCard), nameof(NCard.Reload))]
public static class NCardReloadPatch
{
    [HarmonyPostfix]
    public static void Postfix(NCard __instance)
    {
        CardGemDisplay.UpdateFromNCard(__instance);
    }
}