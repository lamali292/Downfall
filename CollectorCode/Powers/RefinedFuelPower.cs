using Collector.CollectorCode.Core;
using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class RefinedFuelPower : CollectorPowerModel
{
    public RefinedFuelPower()
    {
        WithReserve(1);
        WithTip(CardKeyword.Exhaust);
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    private int StatusExhaustedThisTurn => CombatManager.Instance.History.Entries
        .OfType<CardExhaustedEntry>().Count(e =>
            e.Actor == Owner && e.HappenedThisTurn(CombatState) && e.Card.Type == CardType.Status);

    public override int DisplayAmount => Math.Max(Amount-StatusExhaustedThisTurn, 0);

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature != Owner || card.Type != CardType.Status) return;
        if (StatusExhaustedThisTurn > Amount) return;
        await PowerCmd.Apply<ReserveNextTurnPower>(ctx, Owner, DynamicVars.Reserve.BaseValue, Owner, null);
        InvokeDisplayAmountChanged();
    }
}