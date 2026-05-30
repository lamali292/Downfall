using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Core;

public static class SlimeBossModelDb
{
    public static T Slime<T>() where T : SlimeModel => ModelDb.Get<T>();

    public static IEnumerable<SlimeModel> AllSlimes =>
        ModelDb.AllAbstractModelSubtypes
            .Where(t => t.IsSubclassOf(typeof(SlimeModel)))
            .Select(t => (SlimeModel)ModelDb.Get(t));
    
    
    public static IEnumerable<SlimeModel> AllSpecialistSlimes => AllSlimes.Where(t => t.SlimeType == SlimeType.Specialist);
    public static IEnumerable<SlimeModel> AllNormalSlimes => AllSlimes.Where(t => t.SlimeType == SlimeType.Normal);

    private static Dictionary<Type, CardModel>? _slimeCardByType;

    private static Dictionary<Type, CardModel> SlimeCardByType =>
        _slimeCardByType ??= ModelDb.AllCards
            .Where(c => c.GetType().BaseType is { IsGenericType: true } baseType &&
                        baseType.GetGenericTypeDefinition() == typeof(SlimeCard<>))
            .ToDictionary(c => c.GetType().BaseType!.GenericTypeArguments[0]);

    public static CardModel GetCardForSlime(SlimeModel slime) =>
        SlimeCardByType[slime.GetType()];

    public static SlimeCard<T> GetCardForSlime<T>() where T : SlimeModel =>
        (SlimeCard<T>)SlimeCardByType[typeof(T)];
    
}