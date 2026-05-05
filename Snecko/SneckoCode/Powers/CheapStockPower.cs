using MegaCrit.Sts2.Core.Combat;
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
    
    protected override async Task AfterSideTurnStart(PlayerChoiceContext ctx, CombatSide side, ICombatState combatState)
    {
        if (side != Owner.Side || Owner.Player == null) return;
        var cards = Owner.Player.PlayerCombatState?.Hand.Cards.OrderByDescending(e => e.EnergyCost.GetResolved()).Take(Amount) ?? [];
        await SneckoCmd.Muddle(ctx, cards, this);
    }
    
}