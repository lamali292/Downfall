using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Powers;

// "The next N times you play a Status, gain 1 [E]." Same one-shot-charge shape as vanilla FreeSkillPower.
public class RecyclingPower : SlimeBossPowerModel
{
    public override async Task AfterCardPlayed(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner.Creature != Owner || cardPlay.Card.Type != CardType.Status || Amount <= 0) return;
        await PlayerCmd.GainEnergy(1, Owner.Player!);
        await PowerCmd.Decrement(this);
    }
}
