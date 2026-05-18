using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     At the end of your turn, Retain up to X cards.
///     Wears off at end of turn.
///     Uses the same pattern as WellLaidPlansPower from the base game.
/// </summary>
public sealed class CoalescencePower : HermitPowerModel
{
    private static bool RetainFilter(CardModel card)
    {
        return !card.ShouldRetainThisTurn;
    }

    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player || player.Creature.CombatState == null) return;
        if (!Hook.ShouldFlush(player.Creature.CombatState, player)) return;

        var selected = (await CardSelectCmd.FromHand(
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount),
            context: choiceContext,
            player: Owner.Player,
            filter: RetainFilter,
            source: this
        )).ToList();

        if (selected.Count == 0) return;

        foreach (var card in selected) card.GiveSingleTurnRetain();
    }

    public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side == Owner.Side) await PowerCmd.Remove(this);
    }
}