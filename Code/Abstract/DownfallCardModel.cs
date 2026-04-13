using System.Runtime.CompilerServices;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.Code.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Abstract;

public abstract class DownfallCardModel(
    int cost,
    CardType type,
    CardRarity rarity,
    TargetType targetType)
    : ConstructedCardModel(cost, type, rarity, targetType)
{
    private readonly ConditionalWeakTable<string, PowerModel> _powerCache = new();

    protected ConstructedCardModel WithIcon<T>(string iconKey = "Power")
        where T : PowerModel
    {
        var power = ModelDb.Power<T>();
        _powerCache.Add(iconKey, power);
        return this;
    }

    protected override void AddExtraArgsToDescription(LocString description)
    {
        foreach (var keyValuePair in _powerCache) description.AddObj(keyValuePair.Key, keyValuePair.Value);
    }


    protected async Task<CardModel?> SelectFromHand(PlayerChoiceContext ctx, int count = 1,
        Func<CardModel, bool>? filter = null)
    {
        return (await CardSelectCmd.FromHand(ctx, Owner, new CardSelectorPrefs(SelectionScreenPrompt, count), filter,
            this)).FirstOrDefault();
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
    public override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath<T>();
}