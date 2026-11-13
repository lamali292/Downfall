using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

public class ConsultPlaybookPower : SlimeBossPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return;
        if (cardPlay.Card.Type != CardType.Attack) return;
        var attackPlaysThisTurn = CombatManager.Instance.History.CardPlaysFinished
            .Count(e => e.Actor == Owner && e.CardPlay.Card.Type == CardType.Attack && e.HappenedThisTurn(CombatState));

        if (attackPlaysThisTurn > 1) return;
        if (!SlimeBossCmd.IsTwoEnergyAttack(cardPlay.Card, cardPlay)) return;
        await PlayerCmd.GainEnergy(Amount, Owner.Player!);
        await CardPileCmd.Draw(ctx, Amount, Owner.Player!);
    }
}