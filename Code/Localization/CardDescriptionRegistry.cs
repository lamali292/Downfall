using Downfall.Code.Cards.CardModels;
using Downfall.Code.Core;
using Downfall.Code.Core.Champ;
using Downfall.Code.Extensions;
using Downfall.Code.Interfaces;
using Downfall.Code.Keywords;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Localization;


public interface IExtraDescriptionSource
{
    void AddDescriptionLines(CardModel card, List<string> source);
}

public static class CardDescriptionRegistry
{
    public static void RegisterAll()
    {
        Register<AutomatonCardModel>(new EncodeDescriptionSource());
        Register<AutomatonCardModel>(new CompileDescriptionSource());
        Register<AutomatonCardModel>(new CompileErrorDescriptionSource());
        Register<ChampCardModel>(new ChampDescriptionSource());
    }
    private static readonly Dictionary<Type, List<IExtraDescriptionSource>> Sources = new();
    private static void Register<T>(IExtraDescriptionSource source) where T : CardModel
    {
        if (!Sources.TryGetValue(typeof(T), out var list))
            Sources[typeof(T)] = list = new();
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






