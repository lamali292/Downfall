using BaseLib.Utils;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Core;

public static class SlimeQueue
{
    private static readonly SpireField<Player, int> SlimeSlots = new(_ => 3);

    private static List<Creature> GetSlimes(Player player)
    {
        return player.PlayerCombatState?.Pets.Where(e => e.Monster is SlimeModel).ToList() ?? [];
    }

    public static async Task AddSlime(Player player, SlimeModel slimeModel)
    {
        var maxSlots = SlimeSlots[player];
        var slimes = GetSlimes(player);

        while (slimes.Count >= maxSlots)
        {
            var oldest = slimes[0];
            slimes.RemoveAt(0);
            if (oldest.IsAlive)
                await CreatureCmd.Kill(oldest);
        }

        var pet = player.Creature.CombatState?.CreateCreature(slimeModel.ToMutable(), player.Creature.Side, null);
        if (pet == null) return;
        await PlayerCmd.AddPet(pet, player);

        Callable.From(() => RearrangeSlimeOrbRow(player)).CallDeferred();
    }

    public static Task AddSlime<T>(Player player) where T : SlimeModel
    {
        return AddSlime(player, ModelDb.Monster<T>());
    }


    private static void RearrangeSlimeOrbRow(Player player)
    {
        var playerNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
        if (playerNode == null) return;

        var slimes = player.Creature.Pets.Where(e => e.Monster is SlimeModel).ToList();
        var totalSlimes = slimes.Count;
        if (totalSlimes == 0) return;

        const float maxSpacing = 300f;

        var startPoint = new Vector2(300f, -50f);
        var endPoint = new Vector2(-150f, 200f);
        var apexPoint = new Vector2(400f, 150f);

        var chordLength = startPoint.DistanceTo(apexPoint) + apexPoint.DistanceTo(endPoint);
        var tDeltaPerSpacing = maxSpacing / chordLength;
        var totalRequestedTSpan = (totalSlimes - 1) * tDeltaPerSpacing;

        const float tStart = 0.0f;
        var tEnd = totalRequestedTSpan;
        if (totalRequestedTSpan > 1.0f && totalSlimes > 1) tEnd = 1.0f;

        for (var i = 0; i < totalSlimes; i++)
        {
            var activePet = slimes[i];
            var slimeNode = NCombatRoom.Instance?.GetCreatureNode(activePet);
            if (slimeNode == null) continue;

            var layoutIndex = totalSlimes - 1 - i;

            var t = totalSlimes == 1 ? 0.0f : Mathf.Lerp(tStart, tEnd, (float)layoutIndex / (totalSlimes - 1));
            t = Mathf.Clamp(t, 0.0f, 1.0f);

            var relativeOffset = CalculateQuadraticBezier(startPoint, apexPoint, endPoint, t);

            if (player.Creature.Side == CombatSide.Enemy) relativeOffset.X = -relativeOffset.X;

            var targetGlobalPos = playerNode.GlobalPosition + relativeOffset;
            var targetVisualLocalOffset = targetGlobalPos - slimeNode.GlobalPosition;

            var currentVisualPos = slimeNode.Visuals.Position;

            if (!slimeNode.HasMeta("layout_tween"))
            {
                slimeNode.Visuals.Position = targetVisualLocalOffset;
                slimeNode.UpdateBounds(slimeNode.Visuals);
                currentVisualPos = targetVisualLocalOffset; // Update our starting reference
            }

            if (slimeNode.HasMeta("layout_tween"))
            {
                var oldTween = slimeNode.GetMeta("layout_tween").As<Tween>();
                if (oldTween.IsValid()) oldTween.Kill();
            }

            var layoutTween = slimeNode.CreateTween();
            slimeNode.SetMeta("layout_tween", layoutTween);
            layoutTween.TweenProperty(slimeNode.Visuals, "position", targetVisualLocalOffset, 0.35f)
                .From(currentVisualPos)
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Cubic);

            layoutTween.Parallel().TweenCallback(Callable.From(() => slimeNode.UpdateBounds(slimeNode.Visuals)));
        }
    }

    private static Vector2 CalculateQuadraticBezier(Vector2 p0, Vector2 p1, Vector2 p2, float t)
    {
        var u = 1f - t;
        var tt = t * t;
        var uu = u * u;

        var point = uu * p0 + 2f * u * t * p1 + tt * p2;
        return point;
    }
}