using Champ.ChampCode.Core;
using Downfall.DownfallCode.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Powers;

public class EntangledNextTurnPower : ChampPowerModel
{
    public EntangledNextTurnPower() : base(PowerType.Debuff, PowerStackType.Single)
    {
        WithTip(typeof(EntangledPower));
    }

    public override async Task BeforeHandDraw(Player player, PlayerChoiceContext ctx, ICombatState combatState)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Remove(this);
        await PowerCmd.Apply<EntangledPower>(ctx, Owner, Amount, Applier, null);
    }
}