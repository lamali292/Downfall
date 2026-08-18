using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using Downfall.DownfallCode.DynamicVars;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Extensions;

public static class ConstructedCardModelExtensions
{
    extension(ConstructedCardModel card)
    {
        public ConstructedCardModel WithPower<T>(int baseVal, int upgrade,
            bool showTooltip)
            where T : PowerModel
        {
            card._constructedDynamicVars.Add(new PowerVar<T>(baseVal).WithUpgrade(upgrade));
            if (showTooltip)
                card.WithTips(e => [HoverTipFactory.FromPower<T>(e.DynamicVars.Power<T>().IntValue)]);
            return card;
        }
        
        public ConstructedCardModel WithPower<T>(int baseVal, bool showTooltip)
            where T : PowerModel
        {
            return card.WithPower<T>(baseVal, 0, showTooltip);
        }
        
        public ConstructedCardModel WithGold(int baseVal, int upgradeVal = 0)
        {
            return card.WithVar(new GoldVar(baseVal).WithUpgrade(upgradeVal));
        }

        public ConstructedCardModel WithRepeat(int baseVal, int upgradeVal = 0)
        {
            return card.WithVar(new RepeatVar(baseVal).WithUpgrade(upgradeVal));
        }

        public ConstructedCardModel WithTempHp(int baseValue, int upgrade = 0)
        {
            return card.WithVars(new TempHpVar(baseValue).WithUpgrade(upgrade));
        }

        public ConstructedCardModel WithHpLoss(int baseVal, int upgrade = 0)
        {
            return card.WithVar(new HpLossVar(baseVal).WithUpgrade(upgrade));
        }

        public ConstructedCardModel WithSelfDamage(int baseVal, int upgrade = 0)
        {
            return card.WithVar(new SelfDamageVar(baseVal, DamageProps.cardUnpowered).WithUpgrade(upgrade));
        }

        public ConstructedCardModel WithEnemyDamage(int baseValue, int upgrade = 0)
        {
            return card.WithVars(new EnemyDamageVar(baseValue, DamageProps.monsterMove).WithUpgrade(upgrade));
        }

        public ConstructedCardModel WithUpgradedCardTip<T>(
            Action<T, CardModel>? modifyTipCard = null)
            where T : CardModel
        {
            return card.WithTip(new TooltipSource(card =>
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
            return card.WithTip(new TooltipSource(card =>
            {
                var mutable = ModelDb.Card<T>().ToMutable();
                if (mutable is T obj2) modifyTipCard?.Invoke(obj2, card);
                return HoverTipFactory.FromCard(mutable);
            }));
        }
        
        public ConstructedCardModel WithTip(TooltipSource tooltipSource,
            ConstructedCardModel.UpgradeType upgradeType)
        {
            return upgradeType switch
            {
                ConstructedCardModel.UpgradeType.Add => card.WithTips(c => c.IsUpgraded ? [tooltipSource.Tip(c)] : []),
                ConstructedCardModel.UpgradeType.Remove => card.WithTips(c => !c.IsUpgraded ? [] : [tooltipSource.Tip(c)]),
                ConstructedCardModel.UpgradeType.None => card.WithTip(tooltipSource),
                _ => throw new ArgumentOutOfRangeException(nameof(upgradeType), upgradeType, null)
            };
        }

        public ConstructedCardModel WithTip(TooltipSource tooltipSource, int baseVal,
            int upgrade)
        {
            if (baseVal == 0)
                return upgrade == 0 ? card : card.WithTip(tooltipSource, ConstructedCardModel.UpgradeType.Add);
            return card.WithTip(tooltipSource,
                baseVal + upgrade == 0 ? ConstructedCardModel.UpgradeType.Remove : ConstructedCardModel.UpgradeType.None);
        }

        public ConstructedCardModel WithTip<T>() where T : AbstractModel
        {
            return card.WithTip(typeof(T));
        }
        
        public ConstructedCardModel WithArtist<T>() where T : Artist, new()
        {
            return card.WithTips(_ => [Artist.Get<T>().HoverTip]);
        }
        
        public ConstructedCardModel WithScry(int baseValue, int upgrade = 0)
        {
            return card.WithVars(new ScryVar(baseValue).WithUpgrade(upgrade));
        }
    }
}