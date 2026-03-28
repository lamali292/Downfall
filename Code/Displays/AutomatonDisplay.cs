using Downfall.Code.Cards.Piles;
using Downfall.Code.Cards.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Displays;

public class AutomatonDisplay
{
    private static readonly Dictionary<Player, NSequenceDisplay> Displays = new();
    
    static AutomatonDisplay()
    {
        CombatManager.Instance.CombatEnded += _ =>
        {
            foreach (var d in Displays.Values)
                d.QueueFree();
            Displays.Clear();
        };
    }
    
    public static void Refresh(Player creature)
    {
        Displays.GetValueOrDefault(creature)?.Refresh();
    }
    
        
    public static void Register(Player creature, NSequenceDisplay display)
    {
        if (Displays.TryGetValue(creature, out var old))
            old.QueueFree();
        Displays[creature] = display;
    }
    
    public static async Task AnimateCardToSequence(CardModel card, AutomatonPile pile, Player creature)
    {
     
        var slotIndex = pile.Cards.Count;
        var display = Displays.GetValueOrDefault(creature);
        var targetPos = display?.GetSlotGlobalPosition(slotIndex);
        var cardNode = NCard.FindOnTable(card);
        if (cardNode == null || !targetPos.HasValue) return;
        var vfx = NCombatRoom.Instance?.CombatVfxContainer;
        if (vfx != null)
        {
            var gp = cardNode.GlobalPosition;
            cardNode.Reparent(vfx);
            cardNode.GlobalPosition = gp;
        }

        var tween = cardNode.CreateTween().SetParallel();
        tween.TweenProperty(cardNode, "global_position", targetPos.Value, 0.3f)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
        tween.TweenProperty(cardNode, "scale", Vector2.One * NSequenceDisplay.SequencedCardScale, 0.3f)
            .SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Expo);
        cardNode.ToSignal(tween, Tween.SignalName.Finished);
        cardNode.QueueFree();
    }
}