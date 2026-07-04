using Downfall.DownfallCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.DynamicVars;

public class SelfDamageVar : DamageVar
{
    

    public SelfDamageVar(decimal damage, ValueProp props)
        : base("SelfDamage", damage,  props)
    {
        
    }

    public SelfDamageVar(string name, decimal damage, ValueProp props)
        : base(name, damage, props)
    {
    }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        var originalDamage1 = BaseValue;
        if (runGlobalHooks && card.CombatState != null)
            originalDamage1 =  DownfallHook.ModifySelfDamage(card.CombatState, originalDamage1, card, out _);
        PreviewValue = originalDamage1;
    }
}