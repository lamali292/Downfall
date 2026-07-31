using Downfall.DownfallCode.Compatibility;
using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Hermit.HermitCode.Powers;

public sealed class ComboPower : HermitPowerModel, IModifyCardPlayResultLocation
{
    public override int DisplayAmount => Math.Max(0, Amount - GetInternalData<Data>().DeadOnCardsPlayed);

    public CardLocationCompatiblity ModifyCardPlayResultLocationCompability(CardModel card, bool isAutoPlay,
        ResourceInfo resources, CardLocationCompatiblity cardLocation)
    {
        if (
            GetInternalData<Data>().DeadOnCardsPlayed >= Amount
            || card.Owner.Creature != Owner
            || !HermitCmd.IsDeadOnActive(card)
            || card.Type is CardType.Power
            || card.Keywords.Contains(CardKeyword.Exhaust))
            return cardLocation;

        Flash();
        SetDeadOnCardsPlayed(GetInternalData<Data>().DeadOnCardsPlayed + 1);
        return new CardLocationCompatiblity(card.Owner, PileType.Hand, CardPilePosition.Bottom);
    }

    protected override object InitInternalData()
    {
        return new Data();
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