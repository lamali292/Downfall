using Downfall.Code.Cards.CardModels;
using Downfall.Code.Patches;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Localization;

public interface IExtraDescriptionSource
{
    IEnumerable<string> GetLines(CardModel card);
}

public static class CardDescriptionRegistry
{
    private static readonly Dictionary<Type, List<(DescriptionInjectionPoint point, IExtraDescriptionSource source)>>
        Sources = new();

    public static void RegisterAll()
    {
        Register<AutomatonCardModel>(DescriptionInjectionPoint.AboveMainText, new EncodeDescriptionSource());
        Register<AutomatonCardModel>(DescriptionInjectionPoint.BelowMainText, new CompileDescriptionSource());
        Register<AutomatonCardModel>(DescriptionInjectionPoint.BelowMainText, new CompileErrorDescriptionSource());
        Register<ChampCardModel>(DescriptionInjectionPoint.BelowMainText, new SkillBonusDescriptionSource());
        Register<ChampCardModel>(DescriptionInjectionPoint.BelowMainText, new FinisherDescriptionSource());
    }

    private static void Register<T>(DescriptionInjectionPoint point, IExtraDescriptionSource source) where T : CardModel
    {
        if (!Sources.TryGetValue(typeof(T), out var list))
            Sources[typeof(T)] = list = [];
        list.Add((point, source));
    }
    
    public static string GetJoined(CardModel card, DescriptionInjectionPoint point) =>
        string.Join('\n', GetLines(card, point).Where(l => !string.IsNullOrEmpty(l)));

    public static IEnumerable<string> GetLines(CardModel card, DescriptionInjectionPoint point)
    {
        var hierarchy = new List<Type>();
        var type = card.GetType();
        while (type != null && type != typeof(CardModel))
        {
            hierarchy.Add(type);
            type = type.BaseType;
        }

        hierarchy.Reverse();

        foreach (var t in hierarchy)
        {
            if (!Sources.TryGetValue(t, out var list)) continue;
            foreach (var (p, source) in list)
                if (p == point)
                    foreach (var line in source.GetLines(card))
                        yield return line;
        }
    }
}