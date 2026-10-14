using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Hexaghost.HexaghostCode.Powers;

/// <summary>
///     Like Pale Blue Dot: shows how many more cards need to be Exhausted this turn, and when the threshold is
///     reached applies Draw Cards Next Turn. Counted from the combat history, so the power holds no state.
/// </summary>
public class PremonitionPower : HexaghostPowerModel
{
    public const string ExhaustKey = "Exhaust";
    public const int DefaultExhaustThreshold = 2;

    public PremonitionPower()
    {
        WithVars(new DynamicVar(ExhaustKey, DefaultExhaustThreshold));
    }

    private int ExhaustThreshold => DynamicVars[ExhaustKey].IntValue;

    public void SetExhaustThreshold(decimal threshold)
    {
        DynamicVars[ExhaustKey].BaseValue = threshold;
        InvokeDisplayAmountChanged();
    }

    public override int DisplayAmount => Math.Max(0, ExhaustThreshold - CardsExhaustedThisTurn);

    private int CardsExhaustedThisTurn => CombatManager.Instance.History.Entries
        .OfType<CardExhaustedEntry>()
        .Count(e => e.Actor == Owner && e.HappenedThisTurn(Owner.CombatState));

    public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card,
        bool causedByEthereal)
    {
        if (card.Owner != Owner.Player) return;
        InvokeDisplayAmountChanged();
        // Only the exhaust that reaches the threshold triggers, so it happens once per turn.
        if (CardsExhaustedThisTurn != ExhaustThreshold) return;
        Flash();
        await PowerCmd.Apply<DrawCardsNextTurnPower>(choiceContext, Owner, Amount, Owner, null);
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (participants.Contains(Owner)) InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}
