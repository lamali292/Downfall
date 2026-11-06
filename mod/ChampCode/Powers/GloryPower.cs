using Champ.ChampCode.Core;
using Champ.ChampCode.Stance;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Champ.ChampCode.Powers;

public class GloryPower : ChampPowerModel
{
    public GloryPower()
    {
        WithTips(_ => [ChampModelDb.ChampStance<ChampUltimateStance>().HoverTip]);
    }
    
    // Must run before the turn's energy reset: ResetEnergy reads MaxEnergy (ModifyMaxEnergy) before any
    // AfterEnergyReset/BeforeHandDraw hook fires.
    public override async Task BeforeSideTurnStart(PlayerChoiceContext ctx, CombatSide side,
        IReadOnlyList<Creature> participants, ICombatState combatState)
    {
        if (!participants.Contains(Owner) || Owner.Player == null || Amount < 10) return;
        await ChampCmd.EnterUltimateStance(ctx, Owner.Player, this);
        await PowerCmd.ModifyAmount(ctx, this, -10, Owner, null);
    }
}