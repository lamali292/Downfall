using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Character;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.Code.Abstract;

public abstract class DownfallCardModel<T>(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : CustomCardModel(cost, type, rarity, targetType)
    where T : DownfallCharacterModel
{
    public sealed override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath<T>();
}

public abstract class ChampCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Champ>(cost, type, rarity, targetType);

public abstract class CollectorCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Collector>(cost, type, rarity, targetType);

public abstract class GremlinsCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Gremlins>(cost, type, rarity, targetType);

public abstract class GuardianCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Guardian>(cost, type, rarity, targetType);

public abstract class HexaghostCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Hexaghost>(cost, type, rarity, targetType);

public abstract class SlimeBossCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<SlimeBoss>(cost, type, rarity, targetType);

public abstract class SneckoCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel<Snecko>(cost, type, rarity, targetType);