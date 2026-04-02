using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Character;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Abstract;


public abstract class DownfallCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : ConstructedCardModel(cost, type, rarity, targetType)
{
    protected async Task<CardModel?> SelectFromHand(PlayerChoiceContext ctx, int count = 1, Func<CardModel, bool>? filter = null)
    {
        return (await CardSelectCmd.FromHand(ctx, Owner, new CardSelectorPrefs(SelectionScreenPrompt, count), filter, this)).FirstOrDefault();
    }
}

public abstract class DownfallCardModel<T>(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : DownfallCardModel(cost, type, rarity, targetType)
    where T : DownfallCharacterModel
{
    public sealed override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath<T>();
}


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