using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Events;

namespace Snecko.SneckoCode.Powers;

public class FountainPower : SneckoPowerModel, IAfterOverflowEffect
{
    public FountainPower()
    {
        WithTip(SneckoKeywords.Overflow);
    }

    public async Task AfterOverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay, CardModel card)
    {
        var randomEnemy = card.Owner.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (randomEnemy == null) return;
        await PowerCmd.Apply<VenomPower>(ctx, randomEnemy, Amount, Owner, null);
        Flash();
    }
}