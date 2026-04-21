using BaseLib.Patches.Content;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Keywords;

public class CustomTarget
{
    [CustomEnum] public static TargetType Everyone;
    [CustomEnum] public static TargetType Anyone;
}

[HarmonyPatch(typeof(NCardPlay), "ShowMultiCreatureTargetingVisuals")]
internal class ShowMultiCreatureTargetingVisualsPatch
{
    public static void Postfix(NCardPlay __instance)
    {
        if (__instance.Card == null || __instance.Card.TargetType != CustomTarget.Everyone) return;
        __instance.CardNode?.UpdateVisuals(
            __instance.Card.Pile!.Type,
            CardPreviewMode.MultiCreatureTargeting
        );

        var room = NCombatRoom.Instance;
        if (room == null) return;
        foreach (var creatureNode in room.CreatureNodes) creatureNode.ShowMultiselectReticle();
    }
}

[HarmonyPatch(typeof(NMouseCardPlay), "TargetSelection")]
internal class TargetSelectionPatch
{
    // We use a Prefix to handle the logic for 'Anyone' and skip the vanilla method
    public static bool Prefix(NMouseCardPlay __instance, TargetMode targetMode, ref Task __result)
    {
        if (__instance.Card == null || __instance.Card.TargetType != CustomTarget.Anyone) return true;
        __result = AnyoneTargetSelectionAsync(__instance, targetMode);
        return false;
    }

    private static async Task AnyoneTargetSelectionAsync(NMouseCardPlay __instance, TargetMode targetMode)
    {
        __instance.TryShowEvokingOrbs();
        __instance.CardNode?.CardHighlight.AnimFlash();
        await __instance.SingleCreatureTargeting(targetMode, CustomTarget.Anyone);
    }
}

// TODO: controller support
/*
[HarmonyPatch(typeof(NControllerCardPlay), nameof(NControllerCardPlay.Start))]
 class ControllerStartPatch
{
    public static bool Prefix(NControllerCardPlay __instance)
    {
        var card = __instance.Card;
        if (card == null || __instance.CardNode == null || card.TargetType != CustomTarget.Anyone) return true;
        NDebugAudioManager.Instance?.Play("card_select.mp3");
        NHoverTipSet.Remove(__instance.Holder);
        UnplayableReason reason;
        AbstractModel preventer;
        if (!card.CanPlay(out reason, out preventer))
        {
            __instance.CannotPlayThisCardFtueCheck(card);
            __instance.CancelPlayCard();
            var playerDialogueLine = reason.GetPlayerDialogueLine(preventer);
            if (playerDialogueLine == null)
                return false;
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(NThoughtBubbleVfx.Create(playerDialogueLine.GetFormattedText(), __instance.Card.Owner.Creature, 1.0));
            return false;
        }
        __instance.TryShowEvokingOrbs();
        __instance.CardNode.CardHighlight.AnimFlash();
        __instance.CenterCard();
        TaskHelper.RunSafely(__instance.SingleCreatureTargeting(card.TargetType));
        return false;
    }
}
*/

[HarmonyPatch(typeof(ActionTargetExtensions), nameof(ActionTargetExtensions.IsSingleTarget))]
internal class IsSingleTargetPatch
{
    public static void Postfix(TargetType targetType, ref bool __result)
    {
        if (__result) return;
        if (targetType == CustomTarget.Anyone) __result = true;
    }
}

[HarmonyPatch(typeof(NTargetManager), nameof(NTargetManager.AllowedToTargetCreature))]
internal class AllowedToTargetCreaturePatch
{
    public static bool Prefix(NTargetManager __instance, Creature creature, ref bool __result)
    {
        if (__instance._validTargetsType != CustomTarget.Anyone) return true;
        __result = creature is { IsAlive: true };
        return false;
    }
}

[HarmonyPatch(typeof(NCardPlay), nameof(NCardPlay.TryPlayCard))]
internal class TryPlayCardPatch
{
    public static bool Prefix(NCardPlay __instance, Creature? target)
    {
        var card = __instance.Card;
        if (card == null || card.TargetType != CustomTarget.Anyone) return true;
        if (target == null || __instance.Holder.CardModel == null)
        {
            __instance.CancelPlayCard();
            return false;
        }

        if (!__instance.Holder.CardModel.CanPlayTargeting(target))
        {
            __instance.CannotPlayThisCardFtueCheck(__instance.Holder.CardModel);
            __instance.CancelPlayCard();
            return false;
        }

        __instance._isTryingToPlayCard = true;
        var success = card.TryManualPlay(target);
        __instance._isTryingToPlayCard = false;

        if (success)
        {
            __instance.AutoDisableCannotPlayCardFtueCheck();
            if (__instance.Holder.IsInsideTree())
            {
                var size = __instance.GetViewport().GetVisibleRect().Size;
                __instance.Holder.SetTargetPosition(new Vector2(size.X / 2f, size.Y - __instance.Holder.Size.Y));
            }

            AccessTools.Method(typeof(NCardPlay), "Cleanup").Invoke(__instance, [true]);
            var instance = NCombatRoom.Instance;
            if (instance == null)
                return false;
            instance.Ui.Hand.TryGrabFocus();
        }
        else
        {
            __instance.CancelPlayCard();
        }

        return false;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.CanPlayTargeting))]
internal class CanPlayTargetingPatch
{
    public static bool Prefix(CardModel __instance, Creature? target, ref bool __result)
    {
        if (__instance.TargetType != CustomTarget.Anyone) return true;
        __result = target is { IsAlive: true };
        return false;
    }
}

[HarmonyPatch(typeof(CardModel), nameof(CardModel.IsValidTarget))]
internal class IsValidTargetPatch
{
    public static bool Prefix(CardModel __instance, Creature? target, ref bool __result)
    {
        if (__instance.TargetType != CustomTarget.Anyone) return true;
        __result = target is { IsAlive: true };
        return false;
    }
}