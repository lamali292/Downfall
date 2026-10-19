using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.DynamicVars;

/// <summary>
///     A <see cref="GemVar" /> whose value is gained as Brace: on top of the gem effect modifiers, the preview also
///     includes <see cref="IModifyBraceAmount" /> modifiers (e.g. Baalor's Lordly Plate).
/// </summary>
public class BraceGemVar(int baseValue) : GemVar(baseValue)
{
    public override void UpdateCardPreview(
        CardModel card,
        CardPreviewMode previewMode,
        Creature? target,
        bool runGlobalHooks)
    {
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
        if (runGlobalHooks && card.CombatState != null)
            PreviewValue = GuardianHook.ModifyBraceAmount(card.CombatState, card.Owner, PreviewValue, out _);
    }
}
