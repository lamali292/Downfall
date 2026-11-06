using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Hexaghost.HexaghostCode.DynamicVars;

public class GhostflameVar(string name, decimal baseValue) : DynamicVar(name, baseValue)
{
    public int IntensityValue => Math.Max(0, IntValue + (_owner is GhostflameModel ghostflameModel
        ? HexaghostHook.ModifyGhostflameEffectAdditive(ghostflameModel.Owner.Creature.CombatState!,
            ghostflameModel.Owner, ghostflameModel)
        : 0));

    public virtual void UpdateGhostflamePreview(
        GhostflameModel ghostflameModel,
        bool runGlobalHooks)
    {
        var originalDamage1 = BaseValue;
        if (runGlobalHooks)
            originalDamage1 += HexaghostHook.ModifyGhostflameEffectAdditive(ghostflameModel.Owner.Creature.CombatState!,
                ghostflameModel.Owner, ghostflameModel);
        PreviewValue = Math.Max(0, originalDamage1);
    }

    public override string ToString()
    {
        return $"{PreviewValue}";
    }
}

public class GhostflameDamageVar(decimal baseValue) : GhostflameVar("Damage", baseValue);

public class GhostflameBlockVar(decimal baseValue) : GhostflameVar("Block", baseValue);

public class GhostflameSoulburnVar(decimal baseValue) : GhostflameVar("Soulburn", baseValue)
{
    /// <summary>
    ///     What the flame's Soulburn becomes once <c>PowerCmd.Apply</c> runs the power-amount modifiers on it (Red
    ///     Candle, ...). Display only: applying the flame must still use <see cref="GhostflameVar.IntensityValue" />,
    ///     or those modifiers would be counted twice.
    /// </summary>
    public int DisplayedAmount => Math.Max(0, (int)ModifyAsAppliedPower(IntensityValue));

    public override void UpdateGhostflamePreview(GhostflameModel ghostflameModel, bool runGlobalHooks)
    {
        base.UpdateGhostflamePreview(ghostflameModel, runGlobalHooks);
        if (runGlobalHooks) PreviewValue = Math.Max(0, ModifyAsAppliedPower(PreviewValue));
    }

    private decimal ModifyAsAppliedPower(decimal amount)
    {
        if (_owner is not GhostflameModel { Owner.Creature: { CombatState: { } combatState } giver }) return amount;
        return Hook.ModifyPowerAmountGiven(combatState, ModelDb.Power<SoulBurnPower>(), giver, amount, null, null,
            out _);
    }
}