namespace Downfall.DownfallCode.Compatibility;

using System.Reflection;
using System.Runtime.CompilerServices;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;

public static class CardPlayCompat
{
    private static readonly Type Type = typeof(CardPlay);

    private static readonly PropertyInfo CardProp       = Type.GetProperty("Card")!;
    private static readonly PropertyInfo? PlayerProp    = Type.GetProperty("Player"); // may not exist in old version
    private static readonly PropertyInfo TargetProp     = Type.GetProperty("Target")!;
    private static readonly PropertyInfo ResultPileProp = Type.GetProperty("ResultPile")!;
    private static readonly PropertyInfo ResourcesProp  = Type.GetProperty("Resources")!;
    private static readonly PropertyInfo IsAutoPlayProp = Type.GetProperty("IsAutoPlay")!;
    private static readonly PropertyInfo PlayIndexProp  = Type.GetProperty("PlayIndex")!;
    private static readonly PropertyInfo PlayCountProp  = Type.GetProperty("PlayCount")!;

    public static CardPlay Create(
        CardModel card,
        Creature? target,
        PileType resultPile,
        ResourceInfo resources,
        bool isAutoPlay = true,
        int playIndex = 0,
        int playCount = 0)
    {
        var cardPlay = (CardPlay)RuntimeHelpers.GetUninitializedObject(Type);

        CardProp.SetValue(cardPlay, card);
        PlayerProp?.SetValue(cardPlay, card.Owner);
        TargetProp.SetValue(cardPlay, target);
        ResultPileProp.SetValue(cardPlay, resultPile);
        ResourcesProp.SetValue(cardPlay, resources);
        IsAutoPlayProp.SetValue(cardPlay, isAutoPlay);
        PlayIndexProp.SetValue(cardPlay, playIndex);
        PlayCountProp.SetValue(cardPlay, playCount);

        return cardPlay;
    }
}