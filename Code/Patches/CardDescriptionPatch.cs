using System.Reflection;
using Downfall.Code.Abstract;
using Downfall.Code.Localization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Patches;

public enum DescriptionInjectionPoint
{
    TopOfCard,
    AboveMainText,
    BelowMainText,
    AboveKeywords,
    BottomOfCard
}

[HarmonyPatch]
public static class CardDescriptionPatch
{
    private static readonly Action<CardModel, LocString> AddExtraArgsToDescription =
        AccessTools.MethodDelegate<Action<CardModel, LocString>>(
            AccessTools.Method(typeof(CardModel), "AddExtraArgsToDescription"));

    private static readonly LocString Period = new("card_keywords", "PERIOD");

    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(CardModel), "GetDescriptionForPile",
        [
            typeof(PileType),
            AccessTools.Inner(typeof(CardModel), "DescriptionPreviewType"),
            typeof(Creature)
        ]);
    }

    public static bool Prefix(CardModel __instance, PileType pileType, object previewType, Creature? target,
        ref string __result)
    {
        if (__instance is not DownfallCardModel) return true; // let original run for non-downfall cards
        __result = BuildDescription(__instance, pileType, previewType, target);
        return false;
    }

    private static string BuildDescription(CardModel card, PileType pileType, object previewType, Creature? target)
    {
        var description = LocString(card, pileType, previewType, target);


        var source = new List<string>();

        source.AddRange(CardDescriptionRegistry.GetLines(card, DescriptionInjectionPoint.TopOfCard));

        source.AddRange(from keyword in CardKeywordOrder.beforeDescription
            let flag = keyword switch
            {
                CardKeyword.Retain => card.ShouldRetainThisTurn,
                CardKeyword.Sly => card.IsSlyThisTurn,
                _ => card.Keywords.Contains(keyword)
            }
            where flag
            select GetCardText(keyword));

        source.AddRange(CardDescriptionRegistry.GetLines(card, DescriptionInjectionPoint.AboveMainText));
        source.Add(description.GetFormattedText());
        source.AddRange(CardDescriptionRegistry.GetLines(card, DescriptionInjectionPoint.BelowMainText));

        var dynamicExtraCardText1 = card.Enchantment?.DynamicExtraCardText;
        if (dynamicExtraCardText1 != null)
            source.Add($"[purple]{dynamicExtraCardText1.GetFormattedText()}[/purple]");
        var dynamicExtraCardText2 = card.Affliction?.DynamicExtraCardText;
        if (dynamicExtraCardText2 != null)
            source.Add($"[purple]{dynamicExtraCardText2.GetFormattedText()}[/purple]");


        var enchantedReplayCount = card.GetEnchantedReplayCount();
        if (enchantedReplayCount > 0)
        {
            var locString = new LocString("static_hover_tips", "REPLAY.extraText");
            locString.Add("Times", enchantedReplayCount);
            source.Add(locString.GetFormattedText());
        }

        source.AddRange(CardDescriptionRegistry.GetLines(card, DescriptionInjectionPoint.AboveKeywords));
        source.AddRange(CardKeywordOrder.afterDescription.Intersect(card.Keywords).Select(GetCardText));
        source.AddRange(CardDescriptionRegistry.GetLines(card, DescriptionInjectionPoint.BottomOfCard));

        return string.Join('\n', source.Where(l => !string.IsNullOrEmpty(l)));
    }

    private static LocString LocString(CardModel card, PileType pileType, object previewType, Creature? target)
    {
        var descPreviewType = (int)previewType;

        var description = card.Description;
        card.DynamicVars.AddTo(description);
        AddExtraArgsToDescription(card, description);

        var upgradeDisplay = descPreviewType == 1
            ? UpgradeDisplay.UpgradePreview
            : card.IsUpgraded
                ? UpgradeDisplay.Upgraded
                : UpgradeDisplay.Normal;

        description.Add(new IfUpgradedVar(upgradeDisplay));

        var variable1 = pileType is PileType.Hand or PileType.Play;
        description.Add("OnTable", variable1);

        var variable2 = CombatManager.Instance.IsInProgress && (card.Pile?.IsCombatPile ?? pileType.IsCombatPile());
        description.Add("InCombat", variable2);
        description.Add("IsTargeting", target != null);
        description.Add("TargetType", card.TargetType.ToString());

        var prefix = EnergyIconHelper.GetPrefix(card);
        description.Add("energyPrefix", prefix);
        description.Add("singleStarIcon", "[img]res://images/packed/sprite_fonts/star_icon.png[/img]");

        foreach (var variable3 in description.Variables)
            if (variable3.Value is EnergyVar energyVar)
                energyVar.ColorPrefix = prefix;

        return description;
    }

    private static string GetCardText(CardKeyword keyword)
    {
        var slugify = StringHelper.Slugify(keyword.ToString());
        var title = new LocString("card_keywords", slugify + ".title");
        return $"[gold]{title.GetFormattedText()}[/gold]{Period.GetRawText()}";
    }
}