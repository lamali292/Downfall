using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Powers;

public class GamblePower : SneckoPowerModel
{
    public GamblePower()
    {
        WithTip(SneckoKeywords.Muddle);
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player != Owner.Player || AmountOnTurnStart == 0) return;

        var drawn = await CardPileCmd.Draw(ctx, Amount, player, true);
        await SneckoCmd.Muddle(ctx, drawn, this);

        await PowerCmd.Remove(this);
    }
}