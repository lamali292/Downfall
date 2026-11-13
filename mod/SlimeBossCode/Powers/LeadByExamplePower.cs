using BaseLib.Abstracts;
using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class LeadByExamplePower : SlimeBossPowerModel
{
    private int CardPlayCount => CombatManager.Instance.History.CardPlaysFinished
        .Count(e =>
            e.Actor == Owner
            && e.HappenedThisTurn(CombatState)
            && DownfallCmd.TargetsEnemy(e.CardPlay.Card, e.CardPlay.Target));
    

    protected override int? SecondAmount => CardPlayCount;

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !DownfallCmd.TargetsEnemy(cardPlay.Card, cardPlay.Target) ||
            CardPlayCount > Amount) return;
        await SlimeBossCmd.Command(ctx, cardPlay.Card.Owner, 1, false);
        InvokeDisplayAmountChanged();
    }

    public override Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner)) return Task.CompletedTask;
        this.InvokeSilentDisplayAmountChanged();
        return Task.CompletedTask;
    }
}