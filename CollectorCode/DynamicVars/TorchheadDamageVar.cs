using Collector.CollectorCode.Extensions;
using Downfall.DownfallCode.Compatibility;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
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
    
    
    /// <summary>
    ///     The base damage as it will actually be dealt: halved while Torchhead is downed, so cards show it. A card
    ///     that Kindles (has a Kindle var) revives Torchhead before it attacks, so it shows the full number even
    ///     while Torchhead is down.
    /// </summary>
    private decimal BaseDamageFor(Player? owner, CardModel? card = null)
    {
        var revivesFirst = card != null && card.DynamicVars.ContainsKey("Kindle");
        return owner != null && owner.IsTorchheadDowned && !revivesFirst
            ? Math.Max(1, Math.Floor(BaseValue / 2))
            : BaseValue;
    }

    public void UpdatePowerPreview(
        PowerModel power,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        var baseDamage = BaseDamageFor(power.Owner.PetOwner);
        var originalDamage1 = baseDamage;
        if (runGlobalHooks)
        {
            var combatState = power.CombatState;
            originalDamage1 =  CompatibilityHook.ModifyDamage(combatState.RunState, combatState, target, power.Owner.PetOwner?.Torchhead, baseDamage, Props, null, null, ModifyDamageHookType.All, previewMode, out _);
        }
        PreviewValue = originalDamage1;
    }
    
    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        var baseDamage = BaseDamageFor(card.Owner, card);
        var originalDamage1 = baseDamage;
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
            originalDamage1 = CompatibilityHook.ModifyDamage(card.Owner.RunState, combatState, target, card.Owner.Torchhead, baseDamage, Props, card, null, ModifyDamageHookType.All, previewMode, out _);
        }
        PreviewValue = originalDamage1;
    }
}