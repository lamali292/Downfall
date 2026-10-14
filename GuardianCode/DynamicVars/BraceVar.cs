using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.DynamicVars;

public class BraceVar(int baseValue) : DynamicVar("Brace", baseValue)
{
    // Previews IModifyBraceAmount modifiers (e.g. Baalor's Lordly Plate) on the card text. Like Snecko Skull,
    // only the immediate amount is shown; Brace gained through powers isn't reflected.
    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        var preview = BaseValue;
        if (runGlobalHooks && card.CombatState != null)
            preview = GuardianHook.ModifyBraceAmount(card.CombatState, card.Owner, BaseValue, out _);
        PreviewValue = preview;
    }
}
