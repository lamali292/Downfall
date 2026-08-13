using System.Reflection;
using System.Reflection.Emit;
using Downfall.DownfallCode.Abstract;
using Downfall.DownfallCode.Localization;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Patches;

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
    /// Best-effort identifier for logging. Never throws.
    private static string Name(CardModel? card)
    {
        if (card == null) return "<null>";
        try { return card.ToString() ?? card.GetType().Name; }
        catch { return card.GetType().Name; }
    }

    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(CardModel), "GetDescriptionForPile",
        [
            typeof(PileType),
            AccessTools.Inner(typeof(CardModel), "DescriptionPreviewType"),
            typeof(Creature)
        ]);
    }

    public static void Postfix(CardModel __instance, ref string __result)
    {
        if (__instance is not DownfallCardModel) return;

        try
        {
            var top = CardDescriptionRegistry.GetJoined(__instance, DescriptionInjectionPoint.TopOfCard);
            var bottom = CardDescriptionRegistry.GetJoined(__instance, DescriptionInjectionPoint.BottomOfCard);

            if (!string.IsNullOrEmpty(top))
                __result = top + "\n" + __result;
            if (!string.IsNullOrEmpty(bottom))
                __result = __result + "\n" + bottom;
        }
        catch (Exception e)
        {
            DownfallMainFile.Logger.Error($"Postfix description failed for '{Name(__instance)}': {e}");
        }
    }
    
    
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        var codes = instructions.ToList();

        try
        {
            var joinMethod = typeof(string)
                .GetMethods()
                .First(m => m is { Name: nameof(string.Join), IsGenericMethod: true }
                            && m.GetParameters().Length == 2
                            && m.GetParameters()[0].ParameterType == typeof(char));

            var injectMethod = AccessTools.Method(typeof(CardDescriptionPatch), nameof(Inject));

            for (var i = 0; i < codes.Count; i++)
            {
                // After stloc.s source (local 5) — inject AboveMainText at index 0, BelowMainText as Add
                if (codes[i].opcode == OpCodes.Stloc_S && codes[i].operand is LocalBuilder { LocalIndex: 5 })
                {
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, injectMethod));
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldc_I4, (int)DescriptionInjectionPoint.BelowMainText));
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldloc_S, (byte)5));
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));

                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, injectMethod));
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldc_I4, (int)DescriptionInjectionPoint.AboveMainText));
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldloc_S, (byte)5));
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Ldarg_0));
                    break;
                }

                // Before final Join — inject AboveKeywords
                if (!codes[i].Calls(joinMethod)) continue;
                codes.Insert(i, new CodeInstruction(OpCodes.Call, injectMethod));
                codes.Insert(i, new CodeInstruction(OpCodes.Ldc_I4, (int)DescriptionInjectionPoint.AboveKeywords));
                codes.Insert(i, new CodeInstruction(OpCodes.Ldloc_S, (byte)5));
                codes.Insert(i, new CodeInstruction(OpCodes.Ldarg_0));
                break;
            }

            return codes;
        }
        catch (Exception e)
        {
            DownfallMainFile.Logger.Error($"Description transpiler failed, returning original IL: {e}");
            return instructions;
        }
    }

    public static void Inject(CardModel card, List<string> source, DescriptionInjectionPoint point)
    {
        if (card is not DownfallCardModel || source == null) return;

        try
        {
            var lines = CardDescriptionRegistry.GetLines(card, point)
                .Where(l => !string.IsNullOrEmpty(l))
                .ToList();
            if (lines.Count == 0) return;

            if (point == DescriptionInjectionPoint.AboveMainText)
                source.InsertRange(0, lines);
            else
                source.AddRange(lines);
        }
        catch (Exception e)
        {
            DownfallMainFile.Logger.Error($"Inject({point}) failed for '{Name(card)}': {e}");
        }
    }
}

public static class CardKeywordSubRegistry
{
    private static readonly Dictionary<CardKeyword, List<CardKeyword>> _subKeywords = new();

    public static void Register(CardKeyword parent, CardKeyword sub)
    {
        if (!_subKeywords.TryGetValue(parent, out var list))
            _subKeywords[parent] = list = new List<CardKeyword>();
        list.Add(sub);
    }

    public static string AppendSubs(string text, CardKeyword keyword, CardModel card)
    {
        if (!_subKeywords.TryGetValue(keyword, out var subs)) return text;

        var extras = subs
            .Where(card.Keywords.Contains)
            .Select(s => s.GetCardText())
            .ToList();

        return extras.Count == 0 ? text : text + " " + string.Join(" ", extras);
    }
}

[HarmonyPatch(typeof(CardKeywordExtensions), nameof(CardKeywordExtensions.GetCardText))]
public static class GetCardTextPatch
{
    [ThreadStatic] public static CardModel? CurrentCard;

    public static void Postfix(CardKeyword keyword, ref string __result)
    {
        var card = CurrentCard;
        if (card == null) return;

        try
        {
            __result = CardKeywordSubRegistry.AppendSubs(__result, keyword, card);
        }
        catch (Exception e)
        {
            DownfallMainFile.Logger.Error($"AppendSubs failed for keyword '{keyword}': {e}");
        }
    }
}

[HarmonyPatch]
public static class SetCardContextPatch
{
    private static MethodBase TargetMethod()
    {
        return AccessTools.Method(typeof(CardModel), "GetDescriptionForPile",
        [
            typeof(PileType),
            AccessTools.Inner(typeof(CardModel), "DescriptionPreviewType"),
            typeof(Creature)
        ]);
    }

    public static void Prefix(CardModel __instance, out CardModel? __state)
    {
        __state = GetCardTextPatch.CurrentCard;
        GetCardTextPatch.CurrentCard = __instance;
    }

    public static void Finalizer(CardModel? __state)
    {
        GetCardTextPatch.CurrentCard = __state;
    }
}