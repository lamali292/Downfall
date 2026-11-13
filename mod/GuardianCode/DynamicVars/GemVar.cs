using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.DynamicVars;

public class GemVar : EnergyVar
{
    public GemVar(string name, int baseValue) : base(name, baseValue)
    {
    }

    public GemVar(int baseValue) : base("Gem", baseValue)
    {
    }


    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        var originalDamage1 = BaseValue;
        if (runGlobalHooks && card.CombatState != null && _owner is GemModel gem)
            originalDamage1 = GuardianHook.ModifyGemEffect(card.CombatState, gem, BaseValue, card);
        PreviewValue = originalDamage1;
    }

    public override string ToString()
    {
        return $"{PreviewValue}";
    }
}