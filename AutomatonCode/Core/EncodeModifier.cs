using Automaton.AutomatonCode.Cards.Token;
using Downfall.DownfallCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Core;

public abstract class EncodeModifier : DownfallCardModifier
{
    private static string RemoveEncodeSuffix(string input)
    {
        const string suffix = "_ENCODE";
        return input.EndsWith(suffix) 
            ? input[..^suffix.Length] 
            : input;
    }

    public string Identifier => RemoveEncodeSuffix(Id.Entry); 
    
    protected virtual LocString EncodeLocString
    {
        get
        {
            var loc = new LocString("encode", Identifier + ".encode");
            DynamicVars.AddTo(loc);
            return loc;
        }
    }
    
    
    public override void ModifyDescriptionPost(Creature? target, ref string description)
    {
        if (Owner == null) return;

        foreach (var pair in DynamicVars)
        {
            pair.Value.UpdateCardPreview(Owner, CardPreviewMode.None, target, Owner.CombatState != null);
        }

        var text = EncodeLocString.GetFormattedText();
        var line = BuildEncodeLine(text);

        description = JoinNonEmpty("\n", description, line);
    }
    
    private string BuildEncodeLine(string text)
    {
        if (Owner is FunctionCard)
            return text;

        var title  = new LocString("static_hover_tips", "AUTOMATON-ENCODE.title").GetFormattedText();
        var period = new LocString("card_keywords", "PERIOD").GetFormattedText();
        var suffix = $"[gold]{title}[/gold]{period}";

        return JoinNonEmpty("\n", suffix, text);
    }

    private static string JoinNonEmpty(string separator, params string?[] parts) =>
        string.Join(separator, parts.Where(p => !string.IsNullOrEmpty(p)));
    
    
    public static EncodeModifier? On(CardModel card) =>
        Modifiers(card).OfType<EncodeModifier>().FirstOrDefault();
}


