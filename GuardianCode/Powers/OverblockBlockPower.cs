using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guardian.GuardianCode.Powers;

public class OverblockBlockPower : GuardianPowerModel, IAfterGuardianModeChange
{
    public OverblockBlockPower()
    {
        WithTip(GuardianTip.DefensiveMode);
        WithTip(StaticHoverTip.Block);
    }
    
    
    public async Task AfterGuardianModeChange(PlayerChoiceContext ctx, Player player, GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        if (player.Creature != Owner || newMode is not GuardianDefensiveMode) return;
        var candidates = CombatState.Players.Where(e => e != player).ToList();
        var minBlock = candidates.Min(e => e.Creature.Block);
        var lowest = candidates.Where(e => e.Creature.Block == minBlock).ToList();
        var target = lowest.Count == 1
            ? lowest[0]
            : CombatState.RunState.Rng.CombatTargets.NextItem(lowest);
        if (target == null) return;
        await CreatureCmd.GainBlock(target.Creature, Amount, ValueProp.Unpowered, null);
    }
}