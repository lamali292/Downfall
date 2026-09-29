using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Patches;

/// <summary>
/// <c>NCombatRoom.AddCreature</c> re-spreads every pet of the owner along the player's feet whenever a pet is
/// added. Slimes (and the owner's other pets) sit in fixed grid slots, so for owners with slimes we snapshot
/// the existing pet nodes before the call and put them back afterwards. Osty is untouched by the game's
/// respread for the local player, which is why it never appeared affected.
/// </summary>
[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
public static class SlimePetPositionPatch
{
    public static void Prefix(NCombatRoom __instance, Creature creature,
        out List<(NCreature Node, Vector2 Position, bool Interactable)>? __state)
    {
        __state = null;
        var owner = creature.PetOwner;
        if (owner == null) return;

        var ownedPets = __instance.CreatureNodes.Where(n => n.Entity.PetOwner == owner).ToList();
        if (creature.Monster is not SlimeModel && ownedPets.All(n => n.Entity.Monster is not SlimeModel)) return;

        __state = ownedPets.Select(n => (n, n.Position, n.IsInteractable)).ToList();
    }

    public static void Postfix(List<(NCreature Node, Vector2 Position, bool Interactable)>? __state)
    {
        if (__state == null) return;
        foreach (var (node, position, interactable) in __state)
        {
            node.Position = position;
            node.ToggleIsInteractable(interactable);
        }
    }
}
