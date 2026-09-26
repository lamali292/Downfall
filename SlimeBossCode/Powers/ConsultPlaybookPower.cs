using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;

namespace SlimeBoss.SlimeBossCode.Powers;

public class ConsultPlaybookPower : SlimeBossPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner) return;
        var cardPlaysThisTurn = CombatManager.Instance.History.CardPlaysFinished
            .Count(e => e.Actor == Owner && e.HappenedThisTurn(CombatState));
        
        if (cardPlaysThisTurn > 1) return;
        if (!cardPlay.Card.Tags.Contains(SlimeBossTag.Tackle)) return;
        await PlayerCmd.GainEnergy(Amount, Owner.Player!);
        await CardPileCmd.Draw(ctx, Amount, Owner.Player!);
    }
}