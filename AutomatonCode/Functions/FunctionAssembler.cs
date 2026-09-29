using Automaton.AutomatonCode.Cards.Token;
using Automaton.AutomatonCode.Compile;
using Automaton.AutomatonCode.Core;
using Automaton.AutomatonCode.CustomEnums;
using Automaton.AutomatonCode.Encode;
using Automaton.AutomatonCode.Interfaces;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Automaton.AutomatonCode.Functions;

/// <summary>
///     Turns a list of source cards into a Function: merges their Encode and Compile values into the
///     Function's vars, and describes what the Function does as <see cref="FunctionContribution" />s.
///     This is the only place that connects <see cref="FunctionCard" /> to Encode and Compile.
/// </summary>
public static class FunctionAssembler
{
    private const int CardLevelCompileOrder = 100;
    private const int CompileOrder = 200;

    /// <summary>Every var a Function can carry: one per Encode effect and one per Compile effect.</summary>
    public static IEnumerable<DynamicVar> CanonicalVars =>
        Encodable.Valued.Select(e => e.FunctionDynamicVar)
            .Concat(Compilable.All.SelectMany(c => c.FunctionDynamicVars));

    public static void Assemble(FunctionCard function, IReadOnlyList<CardModel> sourceCards)
    {
        foreach (var canonicalVar in CanonicalVars) function.DynamicVars[canonicalVar.Name].BaseValue = 0;

        if (sourceCards.Count <= 0)
        {
            function.SetAssembly(sourceCards, [], string.Empty);
            return;
        }

        var max = AutomatonCmd.GetMax(sourceCards[0].Owner);
        var index = 1;
        foreach (var sourceCard in sourceCards)
        {
            var position = index == 1 ? FunctionPosition.Start
                : index == max ? FunctionPosition.End
                : FunctionPosition.Middle;
            if (AutomatonCmd.IsEncodable(sourceCard) && sourceCard is IEncodable encodable)
            {
                foreach (var encoding in encodable.Encodings) encoding.ApplyEncode(function, sourceCard, position);
            }

            if (sourceCard.Keywords.Contains(AutomatonKeyword.Compile) && sourceCard is ICompilable compilable)
                foreach (var compilation in compilable.Compilations)
                    compilation.ApplyCompile(function, sourceCard);

            index++;
        }

        function.SetAssembly(sourceCards, BuildContributions(sourceCards), FunctionNaming.GetTitle(sourceCards));
    }

    private static List<FunctionContribution> BuildContributions(IReadOnlyList<CardModel> sourceCards)
    {
        var contributions = new List<FunctionContribution>();

        foreach (var encodable in Encodable.Valued)
            contributions.Add(new FunctionContribution
            {
                Keyword = AutomatonKeyword.Encode,
                Order = encodable.Order,
                ActiveVar = encodable.FunctionDynamicVar.Name,
                Type = encodable.Type,
                Target = encodable.Target,
                EndsSequence = encodable.EndsSequence,
                ForcesSelfTarget = encodable.ForcesSelfTarget,
                GainsBlock = encodable.GainsBlock,
                ShownOnCard = true,
                Play = (fn, ctx, target, cardPlay) => encodable.OnPlay(fn, ctx, target, cardPlay),
                HoverTips = fn => encodable.HoverTips(fn),
                Line = fn => encodable.GetDescription(fn).GetFormattedText()
            });

        // What the source cards' effects change about the Function itself (Retain, a fixed cost, ...).
        var order = CardLevelCompileOrder;
        foreach (var sourceCard in sourceCards)
            if (AutomatonCmd.IsEncodable(sourceCard) && sourceCard is IEncodable encodable)
                foreach (var encoding in encodable.Encodings)
                    if (encoding.GetFunctionNote(sourceCard) != null)
                        contributions.Add(new FunctionContribution
                        {
                            Keyword = AutomatonKeyword.Compile,
                            Order = order++,
                            Line = _ => encoding.GetFunctionNote(sourceCard)?.GetFormattedText()
                        });

        order = CompileOrder;
        foreach (var compilable in Compilable.All)
        {
            if (compilable.MergesOnFunction)
            {
                contributions.Add(new FunctionContribution
                {
                    Keyword = AutomatonKeyword.Compile,
                    Order = order,
                    ActiveVar = compilable.FunctionDynamicVar.Name,
                    HoverTips = fn => compilable.HoverTips(fn),
                    Line = fn => compilable.GetDescription(fn, false).GetFormattedText()
                });
            }
            else
            {
                // Not merged: each source card lists its own line.
                foreach (var sourceCard in sourceCards)
                    if (sourceCard.Keywords.Contains(AutomatonKeyword.Compile) && sourceCard is ICompilable ic && ic.Compilations.Any(c => c.GetType() == compilable.GetType()))
                        contributions.Add(new FunctionContribution
                        {
                            Keyword = AutomatonKeyword.Compile,
                            Order = order,
                            Line = _ => compilable.GetDescription(sourceCard, false).GetFormattedText()
                        });
            }

            order++;
        }

        return contributions.OrderBy(c => c.Order).ToList();
    }
}
