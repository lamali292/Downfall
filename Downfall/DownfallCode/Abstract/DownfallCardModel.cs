using System.Runtime.CompilerServices;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Abstract;

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

    protected ConstructedCardModel WithRepeat(int baseVal, int upgradeVal = 0)
    {
        return WithVar(new RepeatVar(baseVal).WithUpgrade(upgradeVal));
    }


    protected ConstructedCardModel WithUpgradedCardTip<T>(Action<T, CardModel>? action = null)
        where T : CardModel
    {
        return WithTip(new TooltipSource(card =>
        {
            var tip = ModelDb.Card<T>().ToMutable();
            if (card.IsUpgraded) tip.UpgradeInternal();
            if (tip is T t) action?.Invoke(t, card);
            return HoverTipFactory.FromCard(tip);
        }));
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
    //public override string CustomPortraitPath =>
    //    $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath<T>();
    public override string CustomPortraitPath =>
        $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.tres".CardImageAtlasPath<T>();
}