using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Commands;

public class DownfallCreatureCmd
{
    public static async Task<decimal> GainBlock(
        Creature creature,
        CardModel card,
        bool fast = false)
    {
        var amount = card.DynamicVars.Block.BaseValue;
        var props = card.DynamicVars.Block.Props;
        var combatState = creature.CombatState;
        if (CombatManager.Instance.IsOverOrEnding || combatState == null)
            return 0M;
        if (creature.IsDead)
            return 0M;
        await Hook.BeforeBlockGained(combatState, creature, amount, props, card);
        var modifiedAmount = amount;
        modifiedAmount = Hook.ModifyBlock(combatState, creature, modifiedAmount, props, card, null, out var modifiers);
        modifiedAmount = Math.Max(modifiedAmount, 0M);
        await Hook.AfterModifyingBlockAmount(combatState, modifiedAmount, card, null, modifiers);
        if (modifiedAmount > 0M)
        {
            SfxCmd.Play("event:/sfx/block_gain");
            VfxCmd.PlayOnCreatureCenter(creature, "vfx/vfx_block");
            creature.GainBlockInternal(modifiedAmount);
            CombatManager.Instance.History.BlockGained(combatState, creature, (int) modifiedAmount, props, null);
            if (fast)
                await Cmd.CustomScaledWait(0.0f, 0.03f);
            else
                await Cmd.CustomScaledWait(0.1f, 0.25f);
        }
        await Hook.AfterBlockGained(combatState, creature, modifiedAmount, props, card);
        return modifiedAmount;
    }

}