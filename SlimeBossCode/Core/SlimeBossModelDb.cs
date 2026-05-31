using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Core;

public static class SlimeBossModelDb
{
    private static Dictionary<Type, CardModel>? _slimeCardByType;

    public static IEnumerable<SlimeModel> AllSlimes =>
        ModelDb.AllAbstractModelSubtypes
            .Where(t => t.IsSubclassOf(typeof(SlimeModel)))
            .Select(t => (SlimeModel)ModelDb.Get(t));


    public static IEnumerable<SlimeModel> AllSpecialistSlimes =>
        AllSlimes.Where(t => t.SlimeType == SlimeType.Specialist);

    public static IEnumerable<SlimeModel> AllNormalSlimes => AllSlimes.Where(t => t.SlimeType == SlimeType.Normal);

    private static Dictionary<Type, CardModel> SlimeCardByType =>
        _slimeCardByType ??= ModelDb.AllCards
            .Where(c => c.GetType().BaseType is { IsGenericType: true } baseType &&
                        baseType.GetGenericTypeDefinition() == typeof(SlimeCard<>))
            .ToDictionary(c => c.GetType().BaseType!.GenericTypeArguments[0]);

    public static T Slime<T>() where T : SlimeModel
    {
        return ModelDb.Get<T>();
    }

    public static CardModel GetCardForSlime(SlimeModel slime)
    {
        return SlimeCardByType[slime.GetType()];
    }

    public static SlimeCard<T> GetCardForSlime<T>() where T : SlimeModel
    {
        return (SlimeCard<T>)SlimeCardByType[typeof(T)];
    }
}