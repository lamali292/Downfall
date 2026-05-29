using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public sealed class ComboPower : HermitPowerModel
{
    public override int DisplayAmount => Math.Max(0, Amount - GetInternalData<Data>().DeadOnCardsPlayed);

    protected override object InitInternalData()
    {
        return new Data();
    }

    public override (PileType, CardPilePosition) ModifyCardPlayResultPileTypeAndPosition(CardModel card,
        bool isAutoPlay, ResourceInfo resources, PileType pileType, CardPilePosition position)
    {
        if (
            GetInternalData<Data>().DeadOnCardsPlayed >= Amount
            || card.Owner.Creature != Owner
            || card is not IHasDeadOnEffect { IsDeadOn: true }
        )
            return (pileType, position);

        Flash();
        SetDeadOnCardsPlayed(GetInternalData<Data>().DeadOnCardsPlayed + 1);
        return (PileType.Hand, CardPilePosition.Bottom);
    }

    public override Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side) return Task.CompletedTask;

        SetDeadOnCardsPlayed(0);
        return Task.CompletedTask;
    }

    private void SetDeadOnCardsPlayed(int value)
    {
        GetInternalData<Data>().DeadOnCardsPlayed = value;
        InvokeDisplayAmountChanged();
    }

    private class Data
    {
        public int DeadOnCardsPlayed;
    }
}