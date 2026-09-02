using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.DynamicVars;
using Downfall.DownfallCode.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

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

    protected virtual Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        return Task.CompletedTask;
    }


    protected sealed override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (await CardExecutionRegistry.BeforeOnPlayInternal(this, ctx, cardPlay)) return;
        await OnPlayInternal(ctx, cardPlay);
        await CardExecutionRegistry.AfterOnPlayInternal(this, ctx, cardPlay);
    }

    protected ConstructedCardModel WithPower<T>(int baseVal, int upgrade,
        bool showTooltip)
        where T : PowerModel
    {
        _constructedDynamicVars.Add(new PowerVar<T>(baseVal).WithUpgrade(upgrade));
        if (showTooltip)
            WithTips(e => [HoverTipFactory.FromPower<T>(e.DynamicVars.Power<T>().IntValue)]);
        return this;
    }

    protected ConstructedCardModel WithEnchantment<T>(int amount = 1, bool showTooltip = true) where T : EnchantmentModel
    {
        _constructedDynamicVars.Add(new EnchantmentVar<T>(amount));
        return showTooltip ? WithTips(e => HoverTipFactory.FromEnchantment<T>(e.DynamicVars.Enchantment<T>().IntValue)) : this;
    }


    protected ConstructedCardModel WithPower<T>(int baseVal, bool showTooltip)
        where T : PowerModel
    {
        return WithPower<T>(baseVal, 0, showTooltip);
    }

    protected ConstructedCardModel WithGold(int baseVal, int upgradeVal = 0)
    {
        return WithVar(new GoldVar(baseVal).WithUpgrade(upgradeVal));
    }

    protected ConstructedCardModel WithRepeat(int baseVal, int upgradeVal = 0)
    {
        return WithVar(new RepeatVar(baseVal).WithUpgrade(upgradeVal));
    }

    protected ConstructedCardModel WithTempHp(int baseValue, int upgrade = 0)
    {
        return WithVars(new TempHpVar(baseValue).WithUpgrade(upgrade));
    }

    protected ConstructedCardModel WithHpLoss(int baseVal, int upgrade = 0)
    {
        return WithVar(new HpLossVar(baseVal).WithUpgrade(upgrade));
    }

    protected ConstructedCardModel WithSelfDamage(int baseVal, int upgrade = 0)
    {
        return WithVar(new SelfDamageVar(baseVal, DamageProps.cardUnpowered).WithUpgrade(upgrade));
    }

    protected ConstructedCardModel WithEnemyDamage(int baseValue, int upgrade = 0)
    {
        return WithVars(new EnemyDamageVar(baseValue, DamageProps.monsterMove).WithUpgrade(upgrade));
    }

    
    protected ConstructedCardModel WithUpgradeChangingCardTip<T1, T2>(
        Action<T1, CardModel>? modifyBaseTipCard = null,
        Action<T2, CardModel>? modifyUpgradedTipCard = null)
        where T1 : CardModel
        where T2 : CardModel
    {
        return WithTip(new TooltipSource(card =>
        {
            if (card.IsUpgraded)
            {
                var mutable = ModelDb.Card<T2>().ToMutable();
                if (mutable is T2 obj) modifyUpgradedTipCard?.Invoke(obj, card);
                return HoverTipFactory.FromCard(mutable);
            }
            else
            {
                var mutable = ModelDb.Card<T1>().ToMutable();
                if (mutable is T1 obj) modifyBaseTipCard?.Invoke(obj, card);
                return HoverTipFactory.FromCard(mutable);
            }
        }));
    }

    protected ConstructedCardModel WithUpgradedCardTip<T>(
        Action<T, CardModel>? modifyTipCard = null)
        where T : CardModel
    {
        return WithCardTip<T>((hoverCard, card) =>
        {
            hoverCard.UpgradeInternal();
            modifyTipCard?.Invoke(hoverCard, card);
        });
    }
    
    protected ConstructedCardModel WithCardTip<T>(
        Action<T, CardModel>? modifyTipCard = null)
        where T : CardModel
    {
        return WithTip(new TooltipSource(card =>
        {
            var mutable = ModelDb.Card<T>().ToMutable();
            if (mutable is T hoverCard) modifyTipCard?.Invoke(hoverCard, card);
            return HoverTipFactory.FromCard(mutable);
        }));
    }

    protected ConstructedCardModel WithTip(TooltipSource tooltipSource,
        UpgradeType upgradeType)
    {
        return upgradeType switch
        {
            UpgradeType.Add => WithTips(c => c.IsUpgraded ? [tooltipSource.Tip(c)] : []),
            UpgradeType.Remove => WithTips(c => !c.IsUpgraded ? [] : [tooltipSource.Tip(c)]),
            UpgradeType.None => WithTip(tooltipSource),
            _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
        };
    }

    protected ConstructedCardModel WithTip(TooltipSource tooltipSource, int baseVal,
        int upgrade)
    {
        if (baseVal == 0)
            return upgrade == 0 ? this : WithTip(tooltipSource, UpgradeType.Add);
        return WithTip(tooltipSource, baseVal + upgrade == 0 ? UpgradeType.Remove : UpgradeType.None);
    }

    protected ConstructedCardModel WithTip<T>() where T : AbstractModel
    {
        return WithTip(typeof(T));
    }

    protected ConstructedCardModel WithEnchantmentTip<T>(int amount = 1) where T : EnchantmentModel
    {
        return WithTips(e => HoverTipFactory.FromEnchantment<T>(amount));
    }


    protected ConstructedCardModel WithArtist<T>() where T : Artist, new()
    {
        return WithTips(_ => [Artist.Get<T>().HoverTip]);
    }

    protected ConstructedCardModel WithScry(int baseValue, int upgrade = 0)
    {
        return WithVars(new ScryVar(baseValue).WithUpgrade(upgrade));
    }
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