using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public sealed class CoalescencePower : HermitPowerModel
{
    private static bool RetainFilter(CardModel card)
    {
        return !card.ShouldRetainThisTurn;
    }

    public override async Task BeforeFlushLate(PlayerChoiceContext ctx, Player player)
    {
        if (player != Owner.Player || player.Creature.CombatState == null) return;
        if (!Hook.ShouldFlush(player.Creature.CombatState, player)) return;
        var prefs = new CardSelectorPrefs(SelectionScreenPrompt, 0, Amount);
        var selected = (await CardSelectCmd.FromHand(
            ctx,
            Owner.Player,
            prefs,
            RetainFilter,
            this
        )).ToList();
        if (selected.Count == 0) return;
        foreach (var card in selected) card.GiveSingleTurnRetain();
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Side) await PowerCmd.Remove(this);
    }
}