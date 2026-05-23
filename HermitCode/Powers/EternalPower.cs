using BaseLib.Abstracts;
using BaseLib.Extensions;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public sealed class EternalPower : HermitPowerModel, IHasSecondAmount
{
    private int _cardsReducedThisTurn = 4;

    public string GetSecondAmount()
    {
        return $"{_cardsReducedThisTurn}";
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return Task.CompletedTask;
        _cardsReducedThisTurn = 4;
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (_cardsReducedThisTurn <= 0
            || card.Owner.Creature != Owner
            || card.Keywords.Contains(CardKeyword.Unplayable))
            return Task.CompletedTask;
        card.EnergyCost.AddThisTurnOrUntilPlayed(-Amount, true);
        _cardsReducedThisTurn--;
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }
}