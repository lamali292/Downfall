using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.DynamicVars;

public class EnemyDamageVar: DynamicVar
{
    public ValueProp Props { get; set; }

    public EnemyDamageVar(decimal damage, ValueProp props)
        : base("EnemyDamage", damage)
    {
        Props = props;
    }

    public EnemyDamageVar(string name, decimal damage, ValueProp props)
        : base(name, damage)
    {
        Props = props;
    }

    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        var originalDamage1 = BaseValue;
        if (runGlobalHooks)
            originalDamage1 = Hook.ModifyDamage(card.Owner.RunState, card.CombatState, target, target, BaseValue, Props, card, ModifyDamageHookType.All, previewMode, out _);
        PreviewValue = originalDamage1;
    }
}