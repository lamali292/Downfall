using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class LeadByExamplePower : SlimeBossPowerModel, IHasSecondAmount
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { IsEnemy: true } || CardPlayCount > Amount) return;
        await SlimeBossCmd.Command(ctx, cardPlay.Card.Owner, 1);
        Flash();
        this.InvokeSecondAmountChanged();
    }

    protected override Task AfterSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        this.InvokeSecondAmountChanged();
        return Task.CompletedTask;
    }


    private int CardPlayCount => CombatManager.Instance.History.CardPlaysFinished
        .Count(e => 
            e.Actor == Owner 
            && e.HappenedThisTurn(CombatState)
            && e.CardPlay.Target is { IsEnemy: true });

    public string GetSecondAmount() => $"{CardPlayCount}";
}