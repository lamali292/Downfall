using Champ.ChampCode.Core;
using Champ.ChampCode.Stance;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Powers;

public class GloryPower : ChampPowerModel
{
    public GloryPower()
    {
        WithTip(new PowerTooltipSource(_ => ChampModelDb.ChampStance<ChampUltimateStance>().HoverTip));
    }
    
    public override async Task BeforeHandDrawLate(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        if (Amount < 10) return;
        await PowerCmd.Apply<UltimateStancePower>(ctx, Owner, 1, Owner, null);
        await PowerCmd.ModifyAmount(ctx, this, -10, Owner, null);
    }
}