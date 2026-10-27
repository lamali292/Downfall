using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Powers;
using Downfall.DownfallCode.Localization;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Localization;

public class ChantDescriptionSource : IExtraDescriptionSource
{
    public IEnumerable<string> GetLines(CardModel card)
    {
        if (!card.Keywords.Contains(AwakenedKeyword.Chant)) yield break;
        var key = card.Id.Entry + ".chant";
        var hasChanted = ChantCmd.HasChanted(card);
        var loc = new LocString("chants", key);
        loc.Add("Chanted", hasChanted);
        card.DynamicVars.AddTo(loc);
        var chantTitle = new LocString("card_keywords", "AWAKENED-CHANT.card_text");
        chantTitle.Add("Chanted", hasChanted);
        // Same "InCombat" rule the game uses for descriptions: only a card in a combat pile of a running fight is dimmed.
        var inCombat = card is { IsCanonical: false, _owner: not null } && CombatManager.Instance.IsInProgress &&
                       card.Pile is { IsCombatPile: true };
        var active = !inCombat || ChantCmd.WasLastCardPlayedPower(card) || hasChanted;
        var colon = new LocString("card_keywords", "COLON").GetFormattedText();
        var icon = hasChanted ? $"[img]{ModelDb.Power<ChosenVersePower>().CustomPackedSpritePath}[/img] " : "";
        var effect = active ? loc.GetFormattedText() : $"[color=#FFFFFF88]{loc.GetFormattedText()}[/color]";
        var text = $"{icon}{chantTitle.GetFormattedText()}{colon}\n{effect}";
        yield return text;
    }
}