using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.DynamicVars;

/// <summary>
/// A Slime effect that applies <typeparamref name="TPower"/> to a target. Its preview also includes what the
/// game's power-amount modifiers (e.g. Red Candle for Soulburn) add when the slime's owner applies the power.
/// </summary>
public class SlimePowerVar<TPower>(decimal baseValue) : SlimeSecondaryVar(baseValue) where TPower : PowerModel
{
    public override decimal ToPreviewValue(decimal modifiedAmount, SlimeModel slime)
    {
        var combatState = slime.PetOwner.CombatState;
        if (combatState == null) return modifiedAmount;
        return Math.Max(0, Hook.ModifyPowerAmountGiven(combatState, ModelDb.Power<TPower>(), slime.PetOwner,
            modifiedAmount, null, null, out _));
    }
}
