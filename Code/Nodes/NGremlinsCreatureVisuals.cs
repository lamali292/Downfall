using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Nodes;

[GlobalClass]
public partial class NGremlinsCreatureVisuals : NCreatureVisuals
{
    private List<Creature> _gremlins = [];
    private int ActiveGremlinIndex { get; set; }

    public void ArrangeGremlins(List<Creature> gremlins)
    {
        _gremlins = gremlins;
        ApplySlotPositions(false);

        foreach (var node in _gremlins.Select(gremlin => NCombatRoom.Instance?.GetCreatureNode(gremlin)))
            node?.ToggleIsInteractable(true);
        var healthBar = GetParent<NCreature>()
            ?.GetNode<Control>("%HealthBar") // NCreatureStateDisplay
            ?.GetNode<NHealthBar>("%HealthBar"); // NHealthBar inside it
        healthBar?.HpBarContainer.Hide();
    }

    public void SwitchToGremlin(int index)
    {
        if (index < 0 || index >= _gremlins.Count) return;
        ActiveGremlinIndex = index;
        ApplySlotPositions(true);
    }

    public void KillGremlin(int index)
    {
        if (index < 0 || index >= _gremlins.Count) return;
        var node = NCombatRoom.Instance?.GetCreatureNode(_gremlins[index]);
        if (node == null) return;

        var tween = node.CreateTween().SetParallel();
        tween.TweenProperty(node, "modulate", new Color(0, 0, 0, 0), 0.4)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(node, "scale", Vector2.Zero, 0.4)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
        tween.Chain().TweenCallback(Callable.From(() => node.Visible = false));
    }

    private int GetSlot(int gremlinIndex)
    {
        var count = _gremlins.Count;
        return (gremlinIndex - ActiveGremlinIndex + count) % count;
    }

    private static Vector2 GetSlotOffset(int slot)
    {
        return slot == 0 ? Vector2.Zero : new Vector2(-80f - (slot - 1) * 60f, 0f);
    }

    private static float GetSlotScale(int slot)
    {
        return slot == 0 ? 1f : 0.6f;
    }

    private void ApplySlotPositions(bool animated)
    {
        var anchor = GetParent<NCreature>()?.Position ?? GlobalPosition;

        // Build ordered list of living gremlins starting from active
        var living = _gremlins
            .Select((g, i) => (gremlin: g, index: i))
            .Where(x => x.gremlin.IsAlive)
            .OrderBy(x => (x.index - ActiveGremlinIndex + _gremlins.Count) % _gremlins.Count)
            .ToList();

        for (var slot = 0; slot < living.Count; slot++)
        {
            var node = NCombatRoom.Instance?.GetCreatureNode(living[slot].gremlin);
            if (node == null) continue;

            var targetPos = anchor + GetSlotOffset(slot);
            var targetScale = Vector2.One * GetSlotScale(slot);
            node.ZIndex = living.Count - slot;

            if (animated)
            {
                var tween = node.CreateTween().SetParallel();
                tween.TweenProperty(node, "position", targetPos, 0.3)
                    .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
                tween.TweenProperty(node, "scale", targetScale, 0.3)
                    .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
            }
            else
            {
                node.Position = targetPos;
                node.Scale = targetScale;
            }
        }
    }
}