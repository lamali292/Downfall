using BaseLib.Abstracts;
using BaseLib.Extensions;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public sealed class EternalPower : HermitPowerModel, IHasSecondAmount
{
    private const int MaxReductions = 4;

    public string GetSecondAmount() =>
        $"{Math.Max(0, MaxReductions - QualifyingHandDrawsThisTurn())}";

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (!fromHandDraw
            || card.Owner.Creature != Owner
            || card.Keywords.Contains(CardKeyword.Unplayable))
            return Task.CompletedTask;

        // The current draw is already recorded in history, so this count includes it.
        if (QualifyingHandDrawsThisTurn() > MaxReductions)
            return Task.CompletedTask;

        card.EnergyCost.AddThisTurnOrUntilPlayed(-Amount, true);
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }

    private int QualifyingHandDrawsThisTurn() =>
        CombatManager.Instance.History.Entries
            .OfType<CardDrawnEntry>()
            .Count(e => e.HappenedThisTurn(CombatState)
                        && e.FromHandDraw
                        && e.Card.Owner.Creature == Owner
                        && !e.Card.Keywords.Contains(CardKeyword.Unplayable));
}