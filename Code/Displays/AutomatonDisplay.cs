using Downfall.Code.Piles;
using Downfall.Code.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
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
			if (GodotObject.IsInstanceValid(old))
				old.QueueFree();

		Displays[creature] = display;
	}

	public static void SetupAutomatonUi(NCombatRoom combatRoom, Player player)
	{
		var display = NSequenceDisplay.Create(player);
		var vfxContainer = combatRoom.CombatVfxContainer;
		vfxContainer.AddChildSafely(display);

		var creatureNode = combatRoom.GetCreatureNode(player.Creature);
		if (creatureNode != null)
		{
			var globalTopPos = creatureNode.GetTopOfHitbox();
			display.Position = vfxContainer.GetGlobalTransform().AffineInverse() * globalTopPos;
			display.Position += new Vector2(100f, -80f);
		}

		Register(player, display);
		display.Refresh();
	}

	public static async Task AnimateCardToSequence(CardModel card, AutomatonPile pile, Player creature)
	{
		var display = Displays.GetValueOrDefault(creature);
		if (display == null) return;

		var vfx = NCombatRoom.Instance?.CombatVfxContainer;
		if (vfx == null) return;

		var cardNode = NCard.FindOnTable(card);
		if (cardNode == null) return;

		var slotIndex = pile.Cards.Count;
		var targetPos = display.GetSlotGlobalPosition(slotIndex);

		var originalGlobalPos = cardNode.GlobalPosition;
		cardNode.GetParent()?.RemoveChild(cardNode);
		vfx.AddChild(cardNode);
		cardNode.GlobalPosition = originalGlobalPos;

		var finalSizeHalf = cardNode.Size * display.Scale / 2f;
		var centeredTarget = targetPos - finalSizeHalf;

		var tween = cardNode.CreateTween().SetParallel();
		tween.TweenProperty(cardNode, "global_position", centeredTarget, 0.4f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Cubic);

		tween.TweenProperty(cardNode, "scale", display.Scale, 0.4f)
			.SetEase(Tween.EaseType.Out)
			.SetTrans(Tween.TransitionType.Cubic);

		await display.ToSignal(tween, Tween.SignalName.Finished);

		cardNode.QueueFree();
		display.Refresh(true);
	}
}