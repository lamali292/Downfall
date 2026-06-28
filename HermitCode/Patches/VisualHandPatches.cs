using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;

namespace Hermit.HermitCode.Patches;

// TODO: hermit visual hand order fix
[HarmonyPatch(typeof(NPlayerHand), nameof(NPlayerHand.RefreshLayout))]
static class HandOrderEnforcer
{
    static void Prefix(NPlayerHand __instance)
    {
        /*
        if (__instance.HasDraggedHolder || __instance._holdersAwaitingQueue.Count > 0)
            return; // intentional desync states — leave them alone
        foreach (var holder in __instance.Holders)
        {
            var model = holder.CardNode?.Model;
            if (model == null) continue;
            var idx = __instance.GetHandInsertIndex(model);
            if (idx >= 0)a
                __instance.CardHolderContainer.MoveChildSafely(holder, idx);
        }*/
    }
}

[HarmonyPatch(typeof(NCardTransformShineVfx), nameof(NCardTransformShineVfx.UpdateCard))]
static class TransformHandReorder
{
    static void Postfix(NCard cardNode, CardModel endCard)
    {
        /*
        if (endCard.Pile is not { Type: PileType.Hand }) return;
        var hand = NPlayerHand.Instance;
        if (hand == null) return;
        if (hand.HasDraggedHolder || hand._holdersAwaitingQueue.Count > 0) return;
        if (hand.GetCardHolder(endCard) is not NHandCardHolder holder) return;
        if (holder.GetParent() != hand.CardHolderContainer) return;

        var idx = hand.GetHandInsertIndex(endCard);
        if (idx >= 0)
            hand.CardHolderContainer.MoveChildSafely(holder, idx);
        hand.RefreshLayout();
        */
    }
}