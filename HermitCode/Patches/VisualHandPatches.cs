using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Vfx.Cards;

namespace Hermit.HermitCode.Patches;

internal static class HandVisualSync
{
    private static bool _syncing;

    public static void Sync(CardPile? pile)
    {
        if (pile?.Type != PileType.Hand) return;
        var hand = NPlayerHand.Instance;
        if (hand == null) return;
        Sync(hand, pile);
    }

    public static void Sync(NPlayerHand? hand = null)
    {
        hand ??= NPlayerHand.Instance;
        if (hand == null) return;
        var pile = FindHandPile(hand);
        if (pile == null) return;
        Sync(hand, pile);
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
    
    
    private static void Sync(NPlayerHand hand, CardPile pile)
    {
        if (_syncing) return;
        _syncing = true;
        try
        {
            var container = hand.CardHolderContainer;
            var visualIndex = 0;

            foreach (var card in pile.Cards)
            {
                if (hand.GetCardHolder(card) is not NHandCardHolder holder)
                    continue; // no visual holder yet — base game's flow will create/reuse it

                if (holder.GetParent() != container)
                    continue; // e.g. being dragged, selected, or mid-transform

                int currentIndex = ((Node)holder).GetIndex(false);
                if (currentIndex != visualIndex)
                    container.MoveChild(holder, visualIndex);

                visualIndex++;
            }

            hand.RefreshLayout();
        }
        finally
        {
            _syncing = false;
        }
    }
}

[HarmonyPatch(typeof(CardPile), nameof(CardPile.InvokeContentsChanged))]
static class HandContentsChangedPatch
{
    static void Postfix(CardPile __instance)
    {
        HandVisualSync.Sync(__instance);
    }
}

[HarmonyPatch(typeof(NPlayerHand), nameof(NPlayerHand.RefreshLayout))]
static class HandRefreshLayoutPatch
{
    static void Postfix(NPlayerHand __instance)
    {
        HandVisualSync.Sync(__instance);
    }
}

[HarmonyPatch(typeof(NCardTransformShineVfx), nameof(NCardTransformShineVfx.UpdateCard))]
static class TransformShineUpdateCardPatch
{
    static void Postfix(NCard cardNode, CardModel endCard)
    {
        if (endCard.Pile is not { Type: PileType.Hand }) return;
        HandVisualSync.Sync(endCard.Pile);
    }
}