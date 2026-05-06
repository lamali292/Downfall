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
        var randomEnemy = CombatState.HittableEnemies.TakeRandom(1, card.Owner.RunState.Rng.CombatTargets).FirstOrDefault();
        if (randomEnemy == null) return;
        await PowerCmd.Apply<VenomPower>(ctx, randomEnemy, Amount, Owner, null);
        Flash();
    }
}