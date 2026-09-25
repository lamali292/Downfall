using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.DownfallCode.Commands;

/// <summary>
///     Finding and summoning/reviving a player's pet creature. Split out of <see cref="DownfallCmd" />,
///     which owns generic card/creature predicates instead.
/// </summary>
public class PetSummonCmd
{
    public static Creature? GainPet<T>(Player summoner) where T : MonsterModel
    {
        return summoner.Creature.CombatState?.Allies.FirstOrDefault(c => c.Monster is T && c.PetOwner == summoner);
    }

    public static async Task<Creature> Summon<T, T2>(
        PlayerChoiceContext ctx,
        Player summoner,
        int hp,
        AbstractModel? source)
        where T : MonsterModel
        where T2 : PowerModel
    {
        var combatState = summoner.Creature.CombatState;
        var existing = combatState?.Allies.FirstOrDefault(c => c.Monster is T && c.PetOwner == summoner);
        var isReviving = existing is { IsAlive: false };

        if (existing is { IsAlive: true })
        {
            await CreatureCmd.GainMaxHp(existing, hp);
            return existing;
        }

        if (isReviving && existing != null)
        {
            summoner.PlayerCombatState?.AddPetInternal(existing);
        }
        else
        {
            existing = await PlayerCmd.AddPet<T>(summoner);
            var node = NCombatRoom.Instance?.GetCreatureNode(existing);
            var playerNode = NCombatRoom.Instance?.GetCreatureNode(summoner.Creature);

            if (node != null && playerNode != null)
            {
                node.Position = playerNode.Position + new Vector2(250f, -75f);
                node.Modulate = Colors.Transparent;
                node.CreateTween()
                    .TweenProperty(node, "modulate", Colors.White, 0.35)
                    .SetDelay(0.1);
                node.StartReviveAnim();
            }

            await PowerCmd.Apply<T2>(ctx, existing, 1M, null, null);
            node?.TrackBlockStatus(summoner.Creature);
            node?.ToggleIsInteractable(true);
        }

        await CreatureCmd.SetMaxHp(existing, hp);
        await CreatureCmd.Heal(existing, hp, isReviving);

        return existing;
    }
}
