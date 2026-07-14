using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace Downfall.DownfallCode.Patches;

/// <summary>
///     Allows cards in custom displays (stasis, sequence, stash, ...) to be found by
///     NCard.FindOnTable so the engine can animate them when moving to other piles.
///
///     Every lookup is validated: a freed node, a node queued for deletion, a pooled
///     node recycled to another card, or a detached node is treated as stale and
///     removed. Returning a bad node here aborts card plays mid-action
///     (ObjectDisposedException in CardPileCmd) and desyncs multiplayer.
///
///     Call Clear() on combat end: the registry is static and CardModels persist
///     across combats, so leftover entries would serve last combat's dead nodes.
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

    /// <summary>Drop all entries. Hook this to combat end.</summary>
    public static void Clear() => _registry.Clear();

    private static bool IsUsable(CardModel model, NCard node)
    {
        return node != null
               && GodotObject.IsInstanceValid(node)
               && !node.IsQueuedForDeletion()
               && node.Model == model   // pooled node may have been recycled to another card
               && node.IsInsideTree();  // detached nodes have no meaningful GlobalPosition
    }

    public static bool Prefix(CardModel card, ref NCard? __result)
    {
        if (!_registry.TryGetValue(card, out var node)) return true;

        if (!IsUsable(card, node))
        {
            _registry.Remove(card); // self-heal; fall back to the vanilla lookup
            return true;
        }

        __result = node;
        return false;
    }
}