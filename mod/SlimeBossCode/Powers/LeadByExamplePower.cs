using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Powers;

/// <summary>The first Amount times each turn the owner plays a card that targets enemies, Command Bruiser Slime.</summary>
public class LeadByExamplePower : SlimeBossPowerModel
{
    // Derived from combat history rather than a counter, so it needs no reset and also counts plays made
    // before this power was applied this turn.
    private int EnemyTargetingPlaysThisTurn => CombatManager.Instance.History.CardPlaysStarted
        .Count(e => e.Actor == Owner && e.HappenedThisTurn(CombatState) &&
                    DownfallCmd.TargetsEnemy(e.CardPlay.Card, e.CardPlay.Target));

    public override int DisplayAmount => Math.Max(Amount - EnemyTargetingPlaysThisTurn, 0);

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side == Owner.Side) this.InvokeSilentDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var card = cardPlay.Card;
        if (card.Owner.Creature != Owner || !DownfallCmd.TargetsEnemy(card, cardPlay.Target)) return;
        InvokeDisplayAmountChanged();
        // The current play is already in the history, so it is within the first Amount if the count is <= Amount.
        if (EnemyTargetingPlaysThisTurn > Amount) return;
        await SlimeBossCmd.Command<BruiserSlime>(ctx, card.Owner, 1, card);
    }
}
