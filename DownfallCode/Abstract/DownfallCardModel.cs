using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.DownfallCode.Abstract;

public abstract class DownfallCardModel
    : ConstructedCardModel
{
    protected DownfallCardModel(
        int cost,
        CardType type,
        CardRarity rarity,
        TargetType targetType,
        bool showInCardLibrary = true,
        bool autoAdd = true) : base(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    {
        WithTips(e => e is DownfallCardModel { Artist: not null } card ? [card.Artist.HoverTip] : []);
    }

    protected virtual Artist? Artist => null;
}

public abstract class DownfallCardModel<T>(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType,
    bool showInCardLibrary = true,
    bool autoAdd = true)
    : DownfallCardModel(cost, type, rarity, targetType, showInCardLibrary, autoAdd)
    where T : DownfallCharacterModel
{
    public override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".CardImageAtlasPath<T>();
}