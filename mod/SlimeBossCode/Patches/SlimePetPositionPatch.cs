using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Patches;

/// <summary>
/// <c>NCombatRoom.AddCreature</c> re-spreads every pet of the owner along the player's feet whenever a pet is
/// added. The local player's slimes (and their other pets) sit in fixed grid slots, so for the local owner we
/// snapshot the existing pet nodes before the call and put them back afterwards. Osty is untouched by the game's
/// respread for the local player, which is why it never appeared affected. Other players' slimes are left to the
/// game's default pet placement, like any other pet.
/// </summary>
[HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom.AddCreature))]
public static class SlimePetPositionPatch
{
    public static void Prefix(NCombatRoom __instance, Creature creature,
        out List<(NCreature Node, Vector2 Position, bool Interactable)>? __state)
    {
        __state = null;
        var owner = creature.PetOwner;
        if (owner == null || !LocalContext.IsMe(owner)) return;

        var ownedPets = __instance.CreatureNodes.Where(n => n.Entity.PetOwner == owner).ToList();
        if (creature.Monster is not SlimeModel && ownedPets.All(n => n.Entity.Monster is not SlimeModel)) return;

        __state = ownedPets.Select(n => (n, n.Position, n.IsInteractable)).ToList();
    }

    public static void Postfix(NCombatRoom __instance, Creature creature,
        List<(NCreature Node, Vector2 Position, bool Interactable)>? __state)
    {
        if (__state != null)
        {
            foreach (var (node, position, interactable) in __state)
            {
                node.Position = position;
                node.ToggleIsInteractable(interactable);
            }
            return;
        }

        // The game spaces pets by the owner's width, which pushes a second slime under the next player. Other
        // players' slimes instead gather in a small circle around the spot a single pet (e.g. Byrdpip) gets.
        var owner = creature.PetOwner;
        if (owner == null || LocalContext.IsMe(owner)) return;
        var ownerNode = __instance.GetCreatureNode(owner.Creature);
        var slimes = __instance.CreatureNodes
            .Where(n => n.Entity.PetOwner == owner && n.Entity.Monster is SlimeModel).ToList();
        if (ownerNode == null || slimes.Count == 0) return;

        var center = new Vector2(
            ownerNode.Position.X - 20f - CircleLeftShift + slimes[0].Visuals.Bounds.Size.X * 0.5f,
            ownerNode.Position.Y + 10f);
        var radius = CircleBaseRadius + CircleRadiusPerSlime * slimes.Count;
        for (var i = 0; i < slimes.Count; i++)
        {
            if (slimes.Count == 1)
            {
                slimes[i].Position = center;
                continue;
            }

            var angle = Mathf.Tau * i / slimes.Count;
            // Squashed vertically so the circle reads as lying on the ground.
            slimes[i].Position = center + new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius * CircleFlatten);
        }
    }

    /// <summary>Moves the circle left of the single-pet spot so it doesn't sit on Byrdpip or other pets.</summary>
    private const float CircleLeftShift = 70f;
    private const float CircleBaseRadius = 22f;
    private const float CircleRadiusPerSlime = 8f;
    private const float CircleFlatten = 0.5f;
}
