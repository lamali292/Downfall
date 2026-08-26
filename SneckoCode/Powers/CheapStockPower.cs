using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Powers;

public class CheapStockPower : SneckoPowerModel
{
    public CheapStockPower()
    {
        WithTip(SneckoKeywords.Muddle);
    }

    public override async Task AfterSideTurnStart(CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.Player == null) return;
        var cards = Owner.Player.Hand.OrderByDescending(e => e.EnergyCost.GetResolved())
            .Take(Amount);
        var ctx = new BlockingPlayerChoiceContext();
        await SneckoCmd.Muddle(ctx, cards, this);
    }
}