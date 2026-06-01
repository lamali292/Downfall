using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class DuplicatedFormPower : SlimeBossPowerModel, IHasSecondAmount
{
    private int _visualValue;


    private int CardsPlayed => CombatManager.Instance.History.CardPlaysStarted
        .Count(e => e.Actor == Owner && e.CardPlay.IsFirstInSeries &&
                    e.HappenedThisTurn(CombatState) && e.CardPlay.Target is { Side: CombatSide.Enemy });


    public string GetSecondAmount()
    {
        return $"{Math.Min(_visualValue, Amount)}";
    }

    public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
    {
        return card.Owner.Creature != Owner ||
               target is not { Side: CombatSide.Enemy } ||
               CardsPlayed >= Amount
            ? playCount
            : playCount + 1;
    }

    public override Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        _visualValue = 0;
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterModifyingCardPlayCount(CardModel card)
    {
        Flash();
        _visualValue++;
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }
}