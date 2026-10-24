using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

/// <summary>Whenever the owner plays a 2 Energy Attack, they gain Strength.</summary>
public class RecklessnessPower : SlimeBossPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || !SlimeBossCmd.IsTwoEnergyAttack(cardPlay.Card)) return;
        await PowerCmd.Apply<StrengthPower>(ctx, Owner, Amount, Owner, cardPlay.Card);
    }
}
