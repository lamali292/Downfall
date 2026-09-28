using Godot;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Gremlins.GremlinsCode.Vfx;

public partial class NGremlinsCreatureVisuals : NCreatureVisuals
{
    private Creature? _activeGremlin;
    private List<Creature> _gremlins = [];

    private NCreature? _playerNode;

    private List<Creature> _rotation = [];

    public void ArrangeGremlins(IReadOnlyList<Creature> gremlins)
    {
        _gremlins = gremlins.ToList();
        _rotation = _gremlins.ToList();
        _activeGremlin = _gremlins.FirstOrDefault();
        ApplySlotPositions(false);
        foreach (var node in _gremlins.Select(g => NCombatRoom.Instance?.GetCreatureNode(g)))
            node?.ToggleIsInteractable(true);
        if (_activeGremlin != null) HideGremlinBar(_activeGremlin);
        SyncBoundsToActive();
    }

    public void ReviveGremlin(Creature gremlin)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(gremlin);
        if (node == null) return;

        node.Visible = true;
        var tween = node.CreateTween().SetParallel();
        tween.TweenProperty(node, "modulate", Colors.White, 0.4)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(node, "scale", Vector2.One * GetSlotScale(GetSlot(gremlin)), 0.4)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);
    }

    private static void HideGremlinBar(Creature gremlin)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(gremlin);
        node?.GetNode<Control>("%HealthBar")
            ?.GetNode<NHealthBar>("%HealthBar")
            ?.HpBarContainer.Hide();
    }

    private static void ShowGremlinBar(Creature gremlin)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(gremlin);
        node?.GetNode<Control>("%HealthBar")
            ?.GetNode<NHealthBar>("%HealthBar")
            ?.HpBarContainer.Show();
    }

    public void SwitchToGremlin(Creature gremlin, IEnumerable<Creature> rotation)
    {
        if (!_gremlins.Contains(gremlin)) return;
        if (_activeGremlin != null) ShowGremlinBar(_activeGremlin);
        _activeGremlin = gremlin;
        _rotation = rotation.ToList();
        ApplySlotPositions(true);
        HideGremlinBar(_activeGremlin);
        SyncBoundsToActive();
    }


    public static void KillGremlin(Creature gremlin)
    {
        var node = NCombatRoom.Instance?.GetCreatureNode(gremlin);
        if (node == null) return;

        var tween = node.CreateTween().SetParallel();
        tween.TweenProperty(node, "modulate", new Color(0, 0, 0, 0), 0.4)
            .SetEase(Tween.EaseType.Out);
        tween.TweenProperty(node, "scale", Vector2.Zero, 0.4)
            .SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Cubic);
        tween.Chain().TweenCallback(Callable.From(() => node.Visible = false));
    }

    public override void _Ready()
    {
        base._Ready();
        _playerNode = GetParentOrNull<NCreature>()
                      ?? GetParent()?.GetParentOrNull<NCreature>();
    }

    private void SyncBoundsToActive()
    {
        if (_playerNode == null || _activeGremlin == null) return;

        var activeNode = NCombatRoom.Instance?.GetCreatureNode(_activeGremlin);
        if (activeNode == null) return;

        var stateDisplay = _playerNode.GetNodeOrNull<NCreatureStateDisplay>("%HealthBar");
        stateDisplay?.SetCreatureBounds(_playerNode.Hitbox);
    }

    private int GetSlot(Creature gremlin)
    {
        return _rotation.IndexOf(gremlin);
    }

    private static Vector2 GetSlotOffset(int slot)
    {
        return slot == 0 ? Vector2.Zero : new Vector2(-120f - (slot - 1) * 60f, 0f);
    }

    private static float GetSlotScale(int slot)
    {
        return slot == 0 ? 1f : 0.6f;
    }

    private void ApplySlotPositions(bool animated)
    {
        var anchor = _playerNode?.GlobalPosition ?? GlobalPosition;
        var living = _rotation.Where(g => g.IsAlive).ToList();

        for (var slot = 0; slot < living.Count; slot++)
        {
            var node = NCombatRoom.Instance?.GetCreatureNode(living[slot]);
            if (node == null) continue;

            var targetPos = anchor + GetSlotOffset(slot);
            var targetScale = Vector2.One * GetSlotScale(slot);
            node.ZIndex = living.Count - slot;

            if (animated)
            {
                var tween = node.CreateTween().SetParallel();
                tween.TweenProperty(node, "global_position", targetPos, 0.3)
                    .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
                tween.TweenProperty(node, "scale", targetScale, 0.3)
                    .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);

                if (slot == 0)
                    tween.Chain().TweenCallback(Callable.From(SyncBoundsToActive));
            }
            else
            {
                node.GlobalPosition = targetPos;
                node.Scale = targetScale;
            }
        }
    }
}