using BaseLib.Cards.Variables;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.DynamicVars;

/// <summary>
///     A calculated hit-count variable whose preview also runs <c>Hook.ModifyAttackHitCount</c>, so relics that change
///     the number of hits (Thumbscrew, ...) show up in the card text. Newer game builds ship this as
///     <c>CalculatedHitCountVar</c>; older ones don't have it, and the hook itself exists on both, so this version
///     works everywhere. Create it with <c>DownfallCardModel.WithCalculatedHitCountVar</c>.
/// </summary>
public class DownfallCalculatedHitCountVar(string name) : CustomCalculatedVar(name)
{
    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks)
    {
        base.UpdateCardPreview(card, previewMode, target, runGlobalHooks);
        PreviewValue = HitCountPreview.Modify(card, (int)PreviewValue, runGlobalHooks);
    }
}

public static class HitCountPreview
{
    /// <summary>The hit count after hooks such as Thumbscrew's, as the attack itself will apply them.</summary>
    public static decimal Modify(CardModel card, int hits, bool runGlobalHooks)
    {
        if (!runGlobalHooks || card.CombatState == null) return hits;
        return Hook.ModifyAttackHitCount(card.CombatState, DamageCmd.Attack(0m).FromCardCompatibility(card, null), hits);
    }
}

/// <summary>
///     The "Repeat" variable of an attack whose Repeat value is its hit count. Same variable and name as
///     <c>RepeatVar</c> (so <c>DynamicVars.Repeat</c> and the "{Repeat}" card text keep working), but the preview
///     also runs <c>Hook.ModifyAttackHitCount</c> (Thumbscrew, ...). Create it with <c>DownfallCardModel.WithHitCount</c>.
/// </summary>
public class DownfallHitCountVar(int hits) : RepeatVar(hits)
{
    public override void UpdateCardPreview(CardModel card, CardPreviewMode previewMode, Creature? target,
        bool runGlobalHooks)
    {
        PreviewValue = HitCountPreview.Modify(card, IntValue, runGlobalHooks);
    }
}
