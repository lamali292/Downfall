using BaseLib.Abstracts;
using BaseLib.Extensions;
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

public class OverblockBlockPower : GuardianPowerModel, IAfterGuardianModeChange, IHasSecondAmount
{
    public OverblockBlockPower()
    {
        WithTip(GuardianTip.DefensiveMode);
        WithTip(StaticHoverTip.Block);
        WithPower<ThornsPower>(0);
    }

    private int ThornsAmount => DynamicVars.Power<ThornsPower>().IntValue;

    public async Task AfterGuardianModeChange(PlayerChoiceContext ctx, Player player, GuardianModeModel oldMode,
        GuardianModeModel newMode)
    {
        if (player.Creature != Owner || newMode is not GuardianDefensiveMode) return;
        var candidates = player.OtherTeammates;
        var minBlock = candidates.Min(e => e.Creature.Block);
        var lowest = candidates.Where(e => e.Creature.Block == minBlock).ToList();
        var target = lowest.Count == 1
            ? lowest[0]
            : CombatState.RunState.Rng.CombatTargets.NextItem(lowest);
        if (target == null) return;
        await CreatureCmd.GainBlock(target.Creature, Amount, BlockProps.nonCardUnpowered, null);
        await PowerCmd.Apply<ThornsPower>(ctx, target.Creature, ThornsAmount, Owner, null);
    }

    public string GetSecondAmount()
    {
        return $"{ThornsAmount}";
    }


    public void IncrementThorns(decimal value)
    {
        AssertMutable();
        DynamicVars.Power<ThornsPower>().BaseValue += value;
        this.InvokeSecondAmountChanged();
    }
}