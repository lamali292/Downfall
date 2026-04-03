using System.Runtime.CompilerServices;
using Downfall.Code.Character;
using Downfall.Code.Commands;
using Downfall.Code.Displays;
using Downfall.Code.Events;
using Downfall.Code.Vfx;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Downfall.Code.Core;

public class AwakenedModel : AbstractModel
{
    private static readonly ConditionalWeakTable<Player, StrongBox<int>> AwakenMeter = new();
    public override bool ShouldReceiveCombatHooks => true;

    public static bool IsAwakened(Player? player)
    {
        return player != null && AwakenMeter.GetOrCreateValue(player).Value >= 7;
    }

    public override Task BeforeCombatStart()
    {
        AwakenMeter.Clear();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        if (IsAwakened(owner)) return;
        if (cardPlay.Card.Type != CardType.Power) return;
        var meter = AwakenMeter.GetOrCreateValue(owner);
        meter.Value++;
        DownfallMainFile.Logger.Info(meter.Value.ToString());
        if (IsAwakened(owner))
            await AwakenedCmd.Awaken(owner, ctx);
    }

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card is Void) await DownfallHook.OnDrained(choiceContext, card.Owner, 1);
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        if (state == null) return Task.CompletedTask;
        foreach (var player in state.Players)
        {
            if (player.Character is not Awakened) continue;
            AwakenedCmd.GetSpellbook(player)?.Refresh(player);
            var combatRoomNode = NCombatRoom.Instance;
            if (combatRoomNode != null) SetupAwakenedUi(combatRoomNode, player);
        }

        return Task.CompletedTask;
    }

    private static void SetupAwakenedUi(NCombatRoom combatRoom, Player player)
    {
        var display = NSpellbookDisplay.Create(player);
        var vfxContainer = combatRoom.CombatVfxContainer;
        vfxContainer.AddChildSafely(display);
        var creatureNode = combatRoom.GetCreatureNode(player.Creature);
        if (creatureNode != null)
        {
            var globalTopPos = creatureNode.GetTopOfHitbox();
            display.Position = vfxContainer.GetGlobalTransform().AffineInverse() * globalTopPos;
            display.Position += new Vector2(-120f, -80f);
        }

        AwakenedDisplay.Register(player, display);
        display.Refresh();
    }
}