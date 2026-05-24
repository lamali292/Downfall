using BaseLib.Abstracts;
using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using Godot;
using Guardian.GuardianCode.Cards;
using Guardian.GuardianCode.Cards.Abstract;
using Guardian.GuardianCode.Displays;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Piles;
using Guardian.GuardianCode.Powers;
using Guardian.GuardianCode.RestSiteOptions;
using Guardian.GuardianCode.Vfx;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Guardian.GuardianCode.Core;

public class GuardianCombatModel() : CustomSingletonModel(HookType.Combat)
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
        await GuardianCmd.TickAll(player, ctx);
        GuardianDisplay.Refresh(player);
    }

    internal static void SetupGuardianCombatUi(CombatState state)
    {
        var combatRoomNode = NCombatRoom.Instance;
        if (combatRoomNode == null) return;

        foreach (var player in state.Players)
        {
            if (player.Character is not Guardian) continue;
            GuardianDisplay.SetupGuardianUi(combatRoomNode, player);
            if (StasisSlots[player] <= 0)
                StasisSlots.Set(player, 3);
            GuardianDisplay.Refresh(player);
        }
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

    private static void TriggerModeAnimation(Player player)
    {
        Callable.From(() =>
        {
            var creatureNode = NCombatRoom.Instance?.GetCreatureNode(player.Creature);
            if (creatureNode?.Visuals is not NGuardianCreatureVisuals guardianVisuals) return;

            guardianVisuals.IsDefensive = ActiveMode[player] is GuardianDefensiveMode;
            guardianVisuals.OnAnimationTrigger("Idle");
        }).CallDeferred();
    }
}

public class GuardianRunModel() : CustomSingletonModel(HookType.Run)
{
    public override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
    {
        if (options.Any(option => option.OptionId == GemRestSiteOption.Id)) return false;

        var deck = player.GetDeck();
        var hasGems = deck.Any(e => e is IGemCard);
        var hasSlots = deck.Any(e => e is GuardianCardModel { FreeSlots: > 0 });
        options.Add(new GemRestSiteOption(player) { IsEnabled = hasSlots & hasGems });
        return true;
    }
}

[HarmonyPatch(typeof(NCombatUi), nameof(NCombatUi.Activate))]
internal static class GuardianCombatUiActivatePatch
{
    private static void Postfix(CombatState state)
    {
        GuardianCombatModel.SetupGuardianCombatUi(state);
    }
}