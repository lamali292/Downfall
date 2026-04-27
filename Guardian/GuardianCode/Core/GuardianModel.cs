using BaseLib.Abstracts;
using BaseLib.Utils;
using Godot;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Displays;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Interfaces;
using Guardian.GuardianCode.Piles;
using Guardian.GuardianCode.Powers;
using Guardian.GuardianCode.RestSiteOptions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rooms;

namespace Guardian.GuardianCode.Core;

public class GuardianModel() : CustomSingletonModel(true, true)
{
    // SpireFields
    internal static readonly SpireField<Player, GuardianModeModel> ActiveMode =
        new(GuardianModelDb.GuardianMode<GuardianNormalMode>);

    internal static readonly SpireField<Player, int> StasisSlots = new(() => 0);
    internal static readonly SpireField<CardModel, int> StasisCounter = new(_ => 0);

    // Hooks
    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Character is not Guardian || combatState.RoundNumber > 1) return;
        await PowerCmd.Apply<ModeShiftPower>(ctx, player.Creature, 20, player.Creature, null, true);
    }

    public override Task AfterCardChangedPilesLate(CardModel card, PileType oldPileType, AbstractModel? source)
    {
        if (card.Pile != null && card.Pile.Type != GuardianPile.Stasis) return Task.CompletedTask;
        GuardianDisplay.Refresh(card.Owner);
        return Task.CompletedTask;
    }

    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        await StasisTickAll(player, ctx);
        GuardianDisplay.Refresh(player);
    }

    public override Task AfterRoomEntered(AbstractRoom room)
    {
        var state = CombatManager.Instance.DebugOnlyGetState();
        var combatRoomNode = NCombatRoom.Instance;
        if (state == null || combatRoomNode == null) return Task.CompletedTask;
        foreach (var player in state.Players)
        {
            if (player.Character is not Guardian) continue;
            GuardianDisplay.SetupGuardianUi(combatRoomNode, player);
            StasisSlots.Set(player, 3);
            GuardianDisplay.Refresh(player);
        }

        return Task.CompletedTask;
    }

    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (player.Character is not Guardian) return false;
        var gems = PileType.Deck.GetPile(player).Cards.Where(e => e is IGemCard).ToList();
        if (gems.Count == 0) return false;
        options.Add(new GemRestSiteOption(player));
        return true;
    }

    // Internal helpers
    private static async Task StasisTickAll(Player player, PlayerChoiceContext ctx)
    {
        var cards = GuardianPile.Stasis.GetPile(player).Cards.ToList();
        foreach (var card in cards.Where(c => StasisCounter[c] > 0))
        {
            StasisCounter[card]--;
            if (card is ITickCard tickCard) await tickCard.OnTick(ctx);
            if (StasisCounter[card] == 0) await ReturnFromStasis(card, player, ctx);
        }

        GuardianDisplay.Refresh(player);
    }

    internal static async Task ReturnFromStasis(CardModel card, Player player, PlayerChoiceContext ctx)
    {
        if (card.Keywords.Contains(GuardianKeyword.Volatile))
        {
            await CardCmd.Exhaust(ctx, card);
            return;
        }

        await CardPileCmd.Add(card, PileType.Hand.GetPile(player));
        card.EnergyCost.SetUntilPlayed(0);
    }

    private static void TriggerModeAnimation(Player player)
    {
        Callable.From(() =>
        {
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            var animState = creatureNode?.SpineAnimation.GetAnimationState();
            if (animState == null) return;
            animState.GetCurrent(0).SetMixDuration(0.3f);
            creatureNode?.SetAnimationTrigger("Idle");
            animState.GetCurrent(0).SetMixDuration(0.3f);
        }).CallDeferred();
    }

    internal static async Task SetMode(PlayerChoiceContext ctx, Player player, GuardianModeModel newCanonical)
    {
        var current = ActiveMode[player];
        if (current?.GetType() == newCanonical.GetType()) return;
        if (current != null) await current.OnExit();
        var mutable = newCanonical.ToMutable(player);
        ActiveMode[player] = mutable;
        await mutable.OnEnter();
        TriggerModeAnimation(player);
        await GuardianHook.OnGuardianModeChange(player.Creature.CombatState!, ctx, player, current!,
            ActiveMode[player]!);
    }
}