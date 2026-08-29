using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Powers;

public class GloryPower : ChampPowerModel
{
    public GloryPower()
    {
        //WithTips(_ => ChampModelDb.ChampStance<ChampUltimateStance>().HoverTips);
        WithCards(1);
    }

    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        if (Amount < 10) return;
        await ChampCmd.EnterUltimateStance(ctx, player, this);
        await CardPileCmd.Draw(ctx, DynamicVars.Cards.IntValue, player);
        await PowerCmd.ModifyAmount(ctx, this, -10, Owner, null);
    }
}