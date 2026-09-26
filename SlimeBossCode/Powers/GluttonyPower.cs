using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;

namespace SlimeBoss.SlimeBossCode.Powers;

// CSV row 37 reused the name "Gluttony" for a second, unrelated card - renamed to avoid a naming collision
// with the free-Status-cards "Gluttony" power (CSV row 31). See project memory for the full note.
public class GluttonyPower : SlimeBossPowerModel, IAfterConsumeEffect
{
    public async Task AfterConsumeEffect(PlayerChoiceContext ctx, Creature creature, Creature attacker)
    {
        if (attacker != Owner) return;
        await PowerCmd.Apply<DrawCardsNextTurnPower>(ctx, Owner, Amount, Owner, null);
    }
}
