using Automaton.AutomatonCode.Piles;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Automaton.AutomatonCode.Vfx;

public partial class StashQueueDisplay : Control
{
    private static readonly Dictionary<Player, StashQueueDisplay> Displays = new();
    private readonly List<QueueItem> _playQueue = new();
    private CardPile? _stashPile;

    static StashQueueDisplay()
    {
        CombatManager.Instance.CombatEnded += _ =>
        {
            foreach (var d in Displays.Values)
                if (IsInstanceValid(d))
                    d.QueueFree();
            Displays.Clear();
        };
    }

    public static StashQueueDisplay? GetDisplay(Player player)
    {
        return Displays.GetValueOrDefault(player);
    }

    private NCard? GetNCard(CardModel card)
    {
        return _playQueue.FirstOrDefault(i => i.Model == card)?.Card;
    }

    public int GetQueueCount()
    {
        return _playQueue.Count;
    }

    public int GetCardIndex(CardModel card)
    {
        return _playQueue.FindIndex(i => i.Model == card);
    }

    public static NCard? GetCardNode(CardModel card)
    {
        return Displays.Values.Select(display => display.GetNCard(card)).OfType<NCard>().FirstOrDefault();
    }


    public static void SetupFor(NCombatRoom combatRoom, Player player)
    {
        var display = new StashQueueDisplay();
        var vfxContainer = combatRoom.CombatVfxContainer;
        vfxContainer.AddChildSafely(display);

        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        if (creatureNode != null)
        {
            var globalTopPos = creatureNode.GetTopOfHitbox();
            var localPos = vfxContainer.GetGlobalTransform().AffineInverse() * globalTopPos;
            display.Position = localPos;
            display.Position += new Vector2(-100f, 200f);
        }

        Displays[player] = display;
        display.SubscribeToStash(player);
    }

    private void SubscribeToStash(Player player)
    {
        _stashPile = StashPile.Stash.GetPile(player);
        _stashPile.CardAdded += OnCardAdded;
        _stashPile.CardRemoved += OnCardRemoved;
    }

    public override void _ExitTree()
    {
        UnsubscribeFromPile();
        _playQueue.Clear();
    }

    private void UnsubscribeFromPile()
    {
        if (_stashPile == null) return;
        _stashPile.CardAdded -= OnCardAdded;
        _stashPile.CardRemoved -= OnCardRemoved;
        _stashPile = null;
    }

    private void OnCardAdded(CardModel card)
    {
        var child = NCard.Create(card);
        if (child == null) return;
        this.AddChildSafely(child);
        this.MoveChildSafely(child, 0);
        child.UpdateVisuals(StashPile.Stash, CardPreviewMode.Normal);

        var item = new QueueItem { Card = child, Model = card };
        _playQueue.Add(item);
        TweenCardToQueuePosition(item, _playQueue.Count - 1);
    }

    private void OnCardRemoved(CardModel card)
    {
        var index = _playQueue.FindIndex(i => i.Model == card);
        if (index < 0) return;

        var item = _playQueue[index];
        _playQueue.RemoveAt(index);

        if (item.Card != null && IsInstanceValid(item.Card))
        {
            item.CurrentTween?.Kill();
            TweenCardForCancellation(item);
        }

        TweenAllToQueuePosition();
    }


    public void TweenAllToQueuePosition()
    {
        for (var index = 0; index < _playQueue.Count; ++index)
            TweenCardToQueuePosition(_playQueue[index], index);
    }

    public void TweenCardToQueuePosition(QueueItem item, int queueIndex)
    {
        item.CurrentTween?.Kill();
        item.CurrentTween = CreateTween().SetParallel();
        item.CurrentTween.TweenProperty(item.Card, (NodePath)"position",
                GetPositionForQueueIndex(item.Card, queueIndex), 0.35f)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        item.CurrentTween.TweenProperty(item.Card, (NodePath)"scale", GetScaleForQueueIndex(queueIndex), 0.35f)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        item.CurrentTween.TweenProperty(item.Card, (NodePath)"modulate:a", 1f, 0.35f)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        item.CurrentTween.Play();
    }

    public void TweenCardForCancellation(QueueItem item)
    {
        item.CurrentTween?.Kill();
        item.CurrentTween = CreateTween().SetParallel();
        item.CurrentTween.TweenProperty(item.Card, (NodePath)"position:y", 30f, 0.5f)
            .AsRelative().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        item.CurrentTween.TweenProperty(item.Card, (NodePath)"modulate:a", 0.0f, 0.5f)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Cubic);
        item.CurrentTween.Chain().TweenCallback(Callable.From(item.Card.QueueFreeSafely));
        item.CurrentTween.Play();
    }

    public void AnimOut()
    {
        foreach (var play in _playQueue)
        {
            play.CurrentTween?.Kill();
            TweenCardForCancellation(play);
        }

        _playQueue.Clear();
    }

    public Vector2 GetScaleForQueueIndex(int index)
    {
        ++index;
        return (float)(1.0 - index / (double)(index + 1)) * Vector2.One * 0.8f;
    }

    public Vector2 GetPositionForQueueIndex(NCard? card, int index)
    {
        ++index;
        var num = index / (float)(index + 2);
        return Vector2.Zero + Vector2.Left * 200f * num;
    }

    public class QueueItem
    {
        public required NCard Card;
        public Tween? CurrentTween;
        public required CardModel Model;
    }
}