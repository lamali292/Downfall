using System.Runtime.CompilerServices;
using Awakened.AwakenedCode.Displays;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.Piles;
using Awakened.AwakenedCode.Vfx;
using BaseLib.Abstracts;
using Downfall.DownfallCode.Vfx;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Void = MegaCrit.Sts2.Core.Models.Cards.Void;

namespace Awakened.AwakenedCode.Core;

public class AwakenedModel() : CustomSingletonModel(HookType.Combat)
{
    private static readonly ConditionalWeakTable<Player, StrongBox<int>> AwakenMeter = new();
    private static readonly ConditionalWeakTable<Player, StrongBox<bool>> AwakenDispatched = new();
    private static readonly ConditionalWeakTable<CombatState, StrongBox<bool>> InitializedCombats = new();
    private static readonly ConditionalWeakTable<Player, StrongBox<bool>> InitializedSpellbooks = new();

    public static bool IsAwakened(Player? player)
    {
        return player != null && AwakenMeter.GetOrCreateValue(player).Value >= 7;
    }

    public static bool MarkAwakened(Player player)
    {
        var dispatched = AwakenDispatched.GetOrCreateValue(player);
        if (dispatched.Value) return false;

        var meter = AwakenMeter.GetOrCreateValue(player);
        meter.Value = 7;
        dispatched.Value = true;
        StatusBarHelper.SetStatus(player, meter.Value, 7, new Color(0x55FFFFFF));
        return true;
    }

    public override Task BeforeCombatStart()
    {
        AwakenMeter.Clear();
        AwakenDispatched.Clear();
        InitializedCombats.Clear();
        InitializedSpellbooks.Clear();
        return Task.CompletedTask;
    }


    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var owner = cardPlay.Card.Owner;
        if (owner.Character is not Awakened) return;
        if (IsAwakened(owner)) return;
        if (cardPlay.Card.Type != CardType.Power) return;
        var meter = AwakenMeter.GetOrCreateValue(owner);
        meter.Value++;
        StatusBarHelper.SetStatus(cardPlay.Card.Owner, meter.Value, 7, new Color(0x55FFFFFF));
        if (IsAwakened(owner))
            await AwakenedCmd.Awaken(owner, ctx);
    }


    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (card is not Void) return;
        var combatState = card.CombatState ?? card.Owner.Creature.CombatState;
        if (combatState == null) return;
        await AwakenedHook.OnDrained(combatState, choiceContext, card.Owner, 1);
    }

    internal static void SetupAwakenedCombatUi(CombatState state)
    {
        var combatRoomNode = NCombatRoom.Instance;
        if (combatRoomNode == null) return;

        var initialized = InitializedCombats.GetOrCreateValue(state);
        if (initialized.Value) return;

        initialized.Value = true;
        foreach (var player in state.Players)
        {
            AwakenMeter.Remove(player);
            AwakenDispatched.Remove(player);
        }

        foreach (var player in state.Players)
        {
            if (player.Character is not Awakened) continue;
            GetOrInitSpellbook(player);
        }
    }

    internal static AwakenedPile GetOrInitSpellbook(Player player)
    {
        var spellbook = AwakenedCmd.GetSpellbookOrThrow(player);

        var initialized = InitializedSpellbooks.GetOrCreateValue(player);
        if (!initialized.Value)
        {
            spellbook.Refresh(player);
            initialized.Value = true;
        }

        var combatRoom = NCombatRoom.Instance;
        if (combatRoom != null) SetupAwakenedUi(combatRoom, player);

        return spellbook;
    }

    private static void SetupAwakenedUi(NCombatRoom combatRoom, Player player)
    {
        if (AwakenedDisplay.HasDisplay(player)) return;

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

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal static class AwakenedCombatUiActivatePatch
{
    private static void Postfix(CombatState state)
    {
        AwakenedModel.SetupAwakenedCombatUi(state);
    }
}