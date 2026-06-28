using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Core;

public abstract class EncodeModifier : DownfallCardModifier
{
    protected virtual LocString EncodeLocString
    {
        get
        {
            var loc = new LocString("encode", Owner!.Id.Entry + ".encode");
            DynamicVars.AddTo(loc);
            return loc;
        }
    }
    public override void ModifyDescriptionPost(Creature? target, ref string description)
    {
        if (Owner == null) return;
        var title  = new LocString("static_hover_tips", "AUTOMATON-ENCODE.title").GetFormattedText();
        var period = new LocString("card_keywords", "PERIOD").GetFormattedText();
        foreach (var keyValuePair in DynamicVars)
        {
            keyValuePair.Value.UpdateCardPreview(Owner, CardPreviewMode.None, target, Owner.CombatState != null);
        }
        var text = EncodeLocString.GetFormattedText();
        var suffix = $"[gold]{title}[/gold]{period}";
        var line   = string.IsNullOrEmpty(text) ? suffix : $"{text}\n{suffix}";

        description = string.IsNullOrEmpty(description) ? line : $"{line}\n{description}";
    }
    
    
    public static EncodeModifier? On(CardModel card) =>
        Modifiers(card).OfType<EncodeModifier>().FirstOrDefault();
}


