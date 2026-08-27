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
    
    public ConstructedCardModel WithPower<T>(int baseVal, int upgrade,
            bool showTooltip)
            where T : PowerModel
        {
            _constructedDynamicVars.Add(new PowerVar<T>(baseVal).WithUpgrade(upgrade));
            if (showTooltip)
                WithTips(e => [HoverTipFactory.FromPower<T>(e.DynamicVars.Power<T>().IntValue)]);
            return this;
        }
        
        public ConstructedCardModel WithEnchantment<T>(int amount = 1, bool showTooltip = true) where T : EnchantmentModel
        {
            _constructedDynamicVars.Add(new EnchantmentVar<T>(amount));
            if (showTooltip)
                return WithTips(e => HoverTipFactory.FromEnchantment<T>(e.DynamicVars.Enchantment<T>().IntValue));
            return this;
        }

        
        public ConstructedCardModel WithPower<T>(int baseVal, bool showTooltip)
            where T : PowerModel
        {
            return WithPower<T>(baseVal, 0, showTooltip);
        }
        
        public ConstructedCardModel WithGold(int baseVal, int upgradeVal = 0)
        {
            return WithVar(new GoldVar(baseVal).WithUpgrade(upgradeVal));
        }

        public ConstructedCardModel WithRepeat(int baseVal, int upgradeVal = 0)
        {
            return WithVar(new RepeatVar(baseVal).WithUpgrade(upgradeVal));
        }

        public ConstructedCardModel WithTempHp(int baseValue, int upgrade = 0)
        {
            return WithVars(new TempHpVar(baseValue).WithUpgrade(upgrade));
        }

        public ConstructedCardModel WithHpLoss(int baseVal, int upgrade = 0)
        {
            return WithVar(new HpLossVar(baseVal).WithUpgrade(upgrade));
        }

        public ConstructedCardModel WithSelfDamage(int baseVal, int upgrade = 0)
        {
            return WithVar(new SelfDamageVar(baseVal, DamageProps.cardUnpowered).WithUpgrade(upgrade));
        }

        public ConstructedCardModel WithEnemyDamage(int baseValue, int upgrade = 0)
        {
            return WithVars(new EnemyDamageVar(baseValue, DamageProps.monsterMove).WithUpgrade(upgrade));
        }

        public ConstructedCardModel WithUpgradedCardTip<T>(
            Action<T, CardModel>? modifyTipCard = null)
            where T : CardModel
        {
            return WithTip(new TooltipSource(card =>
            {
                var mutable = ModelDb.Card<T>().ToMutable();
                mutable.UpgradeInternal();
                if (mutable is T obj2) modifyTipCard?.Invoke(obj2, card);
                return HoverTipFactory.FromCard(mutable);
            }));
        }
        
        public ConstructedCardModel WithCardTip<T>(
            Action<T, CardModel>? modifyTipCard = null)
            where T : CardModel
        {
            return WithTip(new TooltipSource(card =>
            {
                var mutable = ModelDb.Card<T>().ToMutable();
                if (mutable is T obj2) modifyTipCard?.Invoke(obj2, card);
                return HoverTipFactory.FromCard(mutable);
            }));
        }
        
        public ConstructedCardModel WithTip(TooltipSource tooltipSource,
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

        public ConstructedCardModel WithTip(TooltipSource tooltipSource, int baseVal,
            int upgrade)
        {
            if (baseVal == 0)
                return upgrade == 0 ? this : WithTip(tooltipSource, UpgradeType.Add);
            return WithTip(tooltipSource, baseVal + upgrade == 0 ? UpgradeType.Remove : UpgradeType.None);
        }

        public ConstructedCardModel WithTip<T>() where T : AbstractModel
        {
            return WithTip(typeof(T));
        }
        
        public ConstructedCardModel WithEnchantmentTip<T>(int amount = 1) where T : EnchantmentModel
        {
            return WithTips(e => HoverTipFactory.FromEnchantment<T>(amount));
        }
        
      
        public ConstructedCardModel WithArtist<T>() where T : Artist, new()
        {
            return WithTips(_ => [Artist.Get<T>().HoverTip]);
        }
        
        public ConstructedCardModel WithScry(int baseValue, int upgrade = 0)
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