using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Collector.CollectorCode.DynamicVars;

public class TorchheadDamageVar : DynamicVar
{
   
    public ValueProp Props { get; set; }

    public TorchheadDamageVar(string name, Decimal damage, ValueProp props)
        : base(name, damage)
    {
        Props = props;
    }

    
    public TorchheadDamageVar(decimal damage, ValueProp props) : base("TorchheadDamage", damage)
    {
        Props = props;
    }
    
    
    public void UpdatePowerPreview(
        PowerModel power,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        var originalDamage1 = BaseValue;
        if (runGlobalHooks)
        {
            var combatState = power.CombatState;
            originalDamage1 =  CompatibilityHook.ModifyDamage(combatState.RunState, combatState, target, power.Owner.PetOwner?.Torchhead, BaseValue, Props, null, null, ModifyDamageHookType.All, previewMode, out _);
        }
        PreviewValue = originalDamage1;
    }
    
    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        var originalDamage1 = BaseValue;
        var enchantment = card.Enchantment;
        if (enchantment != null)
        {
            var originalDamage2 = originalDamage1 + enchantment.EnchantDamageAdditive(originalDamage1, this.Props);
            originalDamage1 = originalDamage2 * enchantment.EnchantDamageMultiplicative(originalDamage2, this.Props);
            if (!card.IsEnchantmentPreview)
                EnchantedValue = originalDamage1;
        }
        if (runGlobalHooks)
        {
            var combatState = card.CombatState ?? card.Owner.Creature.CombatState;
            originalDamage1 = CompatibilityHook.ModifyDamage(card.Owner.RunState, combatState, target, card.Owner.Torchhead, BaseValue, Props, card, null, ModifyDamageHookType.All, previewMode, out _);
        }
        PreviewValue = originalDamage1;
    }
}