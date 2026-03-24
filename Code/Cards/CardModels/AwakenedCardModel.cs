using BaseLib.Extensions;
using Downfall.Code.Abstract;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using AwakenedCharacter = Downfall.Code.Character.Awakened;

namespace Downfall.Code.Cards.CardModels;

public abstract class AwakenedCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel(cost, type, rarity, targetType)
{
    public sealed override string PortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath<AwakenedCharacter>();
}