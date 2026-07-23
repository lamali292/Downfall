using System.Reflection;
using Godot;
using HarmonyLib;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;

namespace Hermit.HermitCode.Patches;

internal static class HandVisualSync
{
    private static bool _queued;
    public static bool IsSyncing { get; private set; }

    public static void Queue()
    {
        if (_queued) return;
        _queued = true;
        Callable.From(Run).CallDeferred();
    }

    private static void Run()
    {
        _queued = false;
        if (IsSyncing) return;

        var hand = NPlayerHand.Instance;
        if (hand == null) return;

        var pile = FindHandPile(hand);
        if (pile == null) return;

        if (!pile.Cards.Any(HermitCmd.HasDeadOn)) return;
        
        
        IsSyncing = true;
        try
        {
            var container = hand.CardHolderContainer;
            var visualIndex = 0;

            foreach (var card in pile.Cards)
            {
                if (hand.GetCardHolder(card) is not NHandCardHolder holder) continue;
                if (holder.GetParent() != container) continue;

                if (holder.GetIndex() != visualIndex)
                    SafeMoveChild(container, holder, visualIndex);

                visualIndex++;
            }

            hand.RefreshLayout();
        }
        finally
        {
            IsSyncing = false;
        }
    }

    private static CardPile? FindHandPile(NPlayerHand hand)
    {
        foreach (var holder in hand.ActiveHolders)
        {
            var pile = holder.CardModel?.Pile;
            if (pile?.Type == PileType.Hand) return pile;
        }

        return null;
    }

    private static void SafeMoveChild(Node container, Node holder, int index)
    {
        if (!GodotObject.IsInstanceValid(container) || !GodotObject.IsInstanceValid(holder)) return;
        if (holder.GetParent() != container) return;

        var childCount = container.GetChildCount();
        if (childCount == 0) return;

        container.MoveChild(holder, Mathf.Clamp(index, 0, childCount - 1));
    }
}

[HarmonyPatch]
internal static class HandChangedPatches
{
    private static IEnumerable<MethodBase> TargetMethods()
    {
        yield return AccessTools.Method(typeof(CardPile), nameof(CardPile.InvokeCardAddFinished));
        yield return AccessTools.Method(typeof(CardPile), nameof(CardPile.InvokeCardRemoveFinished));
        yield return AccessTools.Method(typeof(CardPile), nameof(CardPile.InvokeContentsChanged));
    }

    [HarmonyPostfix]
    private static void Postfix(CardPile __instance)
    {
        if (__instance.Type == PileType.Hand) HandVisualSync.Queue();
    }
}

[HarmonyPatch(typeof(NPlayerHand), nameof(NPlayerHand.RefreshLayout))]
internal static class HandRefreshLayoutPatch
{
    private static void Postfix()
    {
        if (!HandVisualSync.IsSyncing) HandVisualSync.Queue();
    }
}

[HarmonyPatch(typeof(NCardTransformShineVfx), nameof(NCardTransformShineVfx.UpdateCard))]
internal static class TransformShineUpdateCardPatch
{
    private static void Postfix(CardModel endCard)
    {
        if (endCard.Pile is { Type: PileType.Hand }) HandVisualSync.Queue();
    }
}