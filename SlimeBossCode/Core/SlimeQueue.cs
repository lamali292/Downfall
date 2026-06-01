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
    private static readonly SpireField<Player, int> SlimeSlots = new(_ => 0);

    private static List<Creature> GetSlimes(Player player)
    {
        return player.PlayerCombatState?.Pets.Where(e => e.Monster is SlimeModel).ToList() ?? [];
    }

    public static void ResetAllSlots()
    {
        SlimeSlots._table.Clear();
    }

    public static void SetSlots(Player player, int amount)
    {
        SlimeSlots[player] = amount;
    }

    public static int GetCount(Player player)
    {
        return player.PlayerCombatState?.Pets.Count(e => e.Monster is SlimeModel) ?? 0;
    }

    public static Task IncreaseSlimeSlots(Player player, int amount)
    {
        SlimeSlots[player] += amount;
        return Task.CompletedTask;
        //Callable.From(() => RearrangeSlimeOrbRow(player)).CallDeferred();
    }

    public static async Task<(int actual, int absorbed)> DecreaseSlimeSlots(Player player, int amount = 1)
    {
        if (SlimeSlots[player] <= 0) return (0, 0);
        var actual = Math.Min(amount, SlimeSlots[player]);
        SlimeSlots[player] -= actual;
        var absorbed = await EvictSlimesDownTo(player, GetSlimes(player), SlimeSlots[player]);
        Callable.From(() => RearrangeSlimeOrbRow(player)).CallDeferred();
        return (actual, absorbed);
    }

    public static async Task<(bool added, int absorbed)> AddSlime(Player player, SlimeModel slimeModel)
    {
        var maxSlots = SlimeSlots[player];
        if (maxSlots == 0) return (false, 0);
        var slimes = GetSlimes(player);
        var absorbed = await EvictSlimesDownTo(player, slimes, maxSlots - 1);

        var pet = player.Creature.CombatState?.CreateCreature(slimeModel.ToMutable(), player.Creature.Side, null);
        if (pet == null) return (false, absorbed);
        await PlayerCmd.AddPet(pet, player);

        Callable.From(() => RearrangeSlimeOrbRow(player)).CallDeferred();
        return (true, absorbed);
    }

    private static async Task<int> EvictSlimesDownTo(Player player, List<Creature> slimes, int maxSlots)
    {
        var evicted = 0;
        while (slimes.Count > maxSlots)
        {
            var oldest = slimes[0];
            slimes.RemoveAt(0);
            if (!oldest.IsAlive) continue;
            await CreatureCmd.Kill(oldest);
            player.PlayerCombatState?._pets.Remove(oldest);
            evicted++;
        }

        return evicted;
    }

    public static Task<(bool added, int evicted)> AddSlime<T>(Player player) where T : SlimeModel
    {
        return AddSlime(player, ModelDb.Monster<T>());
    }

    public static async Task<bool> RemoveLeadingSlime(Player player)
    {
        var slimes = GetSlimes(player);
        if (slimes.Count == 0) return false;

        var leading = slimes[^1];
        if (!leading.IsAlive) return false;

        await CreatureCmd.Kill(leading);
        player.PlayerCombatState?._pets.Remove(leading);

        Callable.From(() => RearrangeSlimeOrbRow(player)).CallDeferred();
        return true;
    }

    public static async Task<int> RemoveAll(Player player)
    {
        var slimes = GetSlimes(player);
        if (slimes.Count == 0) return 0;

        var amount = 0;
        foreach (var slime in slimes.Where(slime => slime.IsAlive))
        {
            await CreatureCmd.Kill(slime);
            player.PlayerCombatState?._pets.Remove(slime);
            amount++;
        }

        Callable.From(() => RearrangeSlimeOrbRow(player)).CallDeferred();
        return amount;
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
                currentVisualPos = targetVisualLocalOffset;
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