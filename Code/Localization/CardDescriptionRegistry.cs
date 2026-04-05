using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Localization;

public interface IExtraDescriptionSource
{
    void AddDescriptionLines(CardModel card, List<string> source);
}

public static class CardDescriptionRegistry
{
    private static readonly Dictionary<Type, List<IExtraDescriptionSource>> Sources = new();

    public static void RegisterAll()
    {
        Register<AutomatonCardModel>(new EncodeDescriptionSource());
        Register<AutomatonCardModel>(new CompileDescriptionSource());
        Register<AutomatonCardModel>(new CompileErrorDescriptionSource());
        Register<ChampCardModel>(new FinisherDescriptionSource());
    }

    private static void Register<T>(IExtraDescriptionSource source) where T : CardModel
    {
        if (!Sources.TryGetValue(typeof(T), out var list))
            Sources[typeof(T)] = list = [];
        list.Add(source);
    }

    public static void InjectLines(CardModel card, List<string> source)
    {
        var type = card.GetType();
        while (type != null && type != typeof(CardModel))
        {
            if (Sources.TryGetValue(type, out var list))
                foreach (var s in list)
                    s.AddDescriptionLines(card, source);
            type = type.BaseType;
        }
    }
}