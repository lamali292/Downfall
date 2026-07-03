using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Compatibility;

public static class CompatibilityHook
{
    public static decimal ModifyDamage(
        IRunState runState,
        ICombatState? combatState,
        Creature? target,
        Creature? dealer,
        decimal damage,
        ValueProp props,
        CardModel? cardSource,
        CardPlay? cardPlay,
        ModifyDamageHookType modifyDamageHookType,
        CardPreviewMode previewMode,
        out IEnumerable<AbstractModel> modifiers)
    {
#if V107
        return Hook.ModifyDamage(runState, combatState, target, dealer, damage, props, cardSource, modifyDamageHookType,
            previewMode, out modifiers);
#else
        return Hook.ModifyDamage(runState, combatState, target, dealer, damage, props, cardSource, cardPlay, modifyDamageHookType,
            previewMode, out modifiers);
#endif
    }
}