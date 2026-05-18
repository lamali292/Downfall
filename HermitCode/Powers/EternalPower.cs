using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     First X playable cards drawn at the start of each turn cost 1 less that turn.
/// </summary>
public sealed class EternalPower : HermitPowerModel
{
    public override int DisplayAmount => Math.Max(0, Amount - GetInternalData<Data>().cardsReducedThisTurn);

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
    {
        if (side != Owner.Side) return Task.CompletedTask;

        SetCardsReducedThisTurn(0);
        return Task.CompletedTask;
    }

    private void SetCardsReducedThisTurn(int value)
    {
        GetInternalData<Data>().cardsReducedThisTurn = value;
        InvokeDisplayAmountChanged();
    }


    public override Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel card, bool fromHandDraw)
    {
        if (
            GetInternalData<Data>().cardsReducedThisTurn >= Amount
            || card.Owner.Creature != Owner
            || card.Keywords.Contains(CardKeyword.Unplayable)
        )
            return Task.CompletedTask;

        card.EnergyCost.AddThisTurnOrUntilPlayed(-1, true);
        SetCardsReducedThisTurn(GetInternalData<Data>().cardsReducedThisTurn + 1);
        return Task.CompletedTask;
    }

    private class Data
    {
        public int cardsReducedThisTurn;
    }
}