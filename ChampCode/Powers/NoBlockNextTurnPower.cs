using Champ.ChampCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Champ.ChampCode.Powers;

public class NoBlockNextTurnPower : ChampPowerModel
{
    public NoBlockNextTurnPower() : base(PowerType.Debuff, PowerStackType.Single)
    {
        this.WithTip<NoBlockPower>();
        WithTip(StaticHoverTip.Block);
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<NoBlockPower>(ctx, Owner, Amount, Applier, null);
    }
}