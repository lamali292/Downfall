using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.Code.Patches;

/// <summary>
///     Allows cards in the sequence display to be found by CardPileCmd.FindOnTable
///     so the engine can animate them when moving to other piles.
/// </summary>
[HarmonyPatch(typeof(NCard), nameof(NCard.FindOnTable))]
public static class FindOnTablePatch
{
    private static readonly Dictionary<CardModel, NCard> _registry = new();

    public static void Register(CardModel model, NCard card)
    {
        _registry[model] = card;
    }

    public static void Unregister(CardModel model)
    {
        _registry.Remove(model);
    }

    public static bool Prefix(CardModel card, ref NCard? __result)
    {
        if (!_registry.TryGetValue(card, out var node)) return true;
        __result = node;
        return false;
    }
}