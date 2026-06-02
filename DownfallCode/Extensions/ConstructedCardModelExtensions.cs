using BaseLib.Abstracts;
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
    public static ConstructedCardModel WithPower<T>(this ConstructedCardModel card, int baseVal, int upgrade,
        bool showTooltip)
        where T : PowerModel
    {
        card.WithVar(new DynamicVar(typeof(T).Name, baseVal).WithUpgrade(upgrade));
        if (showTooltip)
            card.WithTips(e => [HoverTipFactory.FromPower<T>(e.DynamicVars.Power<T>().IntValue)]);
        return card;
    }

    public static ConstructedCardModel WithPower<T>(this ConstructedCardModel card, int baseVal, bool showTooltip)
        where T : PowerModel
    {
        return card.WithPower<T>(baseVal, 0, showTooltip);
    }

    public static ConstructedCardModel WithGold(this ConstructedCardModel card, int baseVal, int upgradeVal = 0)
    {
        return card.WithVar(new GoldVar(baseVal).WithUpgrade(upgradeVal));
    }

    public static ConstructedCardModel WithRepeat(this ConstructedCardModel card, int baseVal, int upgradeVal = 0)
    {
        return card.WithVar(new RepeatVar(baseVal).WithUpgrade(upgradeVal));
    }

    public static ConstructedCardModel WithTempHp(this ConstructedCardModel card, int baseValue, int upgrade = 0)
    {
        return card.WithVars(new TempHpVar(baseValue).WithUpgrade(upgrade));
    }

    public static ConstructedCardModel WithHpLoss(this ConstructedCardModel card, int baseVal, int upgrade = 0)
    {
        return card.WithVar(new HpLossVar(baseVal).WithUpgrade(upgrade));
    }

    public static ConstructedCardModel WithSelfDamage(this ConstructedCardModel card, int baseVal, int upgrade = 0)
    {
        return card.WithVar(new DamageVar("SelfDamage", baseVal, ValueProp.Unpowered).WithUpgrade(upgrade));
    }

    public static ConstructedCardModel WithEnemyDamage(this ConstructedCardModel card, int baseValue, int upgrade = 0)
    {
        return card.WithVars(new EnemyDamageVar(baseValue, ValueProp.Unpowered).WithUpgrade(upgrade));
    }

    public static ConstructedCardModel WithUpgradedCardTip<T>(this ConstructedCardModel cons,
        Action<T, CardModel>? modifyTipCard = null)
        where T : CardModel
    {
        return cons.WithTip(new TooltipSource(card =>
        {
            var mutable = ModelDb.Card<T>().ToMutable();
            mutable.UpgradeInternal();
            if (mutable is T obj2) modifyTipCard?.Invoke(obj2, card);
            return HoverTipFactory.FromCard(mutable);
        }));
    }


    public static ConstructedCardModel WithTip(this ConstructedCardModel card, TooltipSource tooltipSource,
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

    public static ConstructedCardModel WithTip(this ConstructedCardModel card, TooltipSource tooltipSource, int baseVal,
        int upgrade)
    {
        if (baseVal == 0)
            return upgrade == 0 ? card : card.WithTip(tooltipSource, ConstructedCardModel.UpgradeType.Add);
        return card.WithTip(tooltipSource,
            baseVal + upgrade == 0 ? ConstructedCardModel.UpgradeType.Remove : ConstructedCardModel.UpgradeType.None);
    }

    public static ConstructedCardModel WithTip<T>(this ConstructedCardModel card) where T : AbstractModel
    {
        return card.WithTip(typeof(T));
    }


    public static ConstructedCardModel WithArtist<T>(this ConstructedCardModel card) where T : Artist, new()
    {
        return card.WithTips(_ => [Artist.Get<T>().HoverTip]);
    }
    
    
    public static ConstructedCardModel WithScry(this ConstructedCardModel card, int baseValue, int upgrade = 0)
    {
        return card.WithVars(new ScryVar(baseValue).WithUpgrade(upgrade));
    }
}