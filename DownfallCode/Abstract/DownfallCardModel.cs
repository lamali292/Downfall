using System.Runtime.CompilerServices;
using BaseLib.Abstracts;
using BaseLib.Extensions;
using Downfall.DownfallCode.DynamicVars;
using Downfall.DownfallCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

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

    protected ConstructedCardModel WithPower<T>(int baseVal, int upgrade, bool showTooltip)
        where T : PowerModel
    {
        WithVar(new PowerVar<T>(baseVal).WithUpgrade(upgrade));
        if (showTooltip)
            WithTip(typeof(T));
        return this;
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
        WithVars(new TempHpVar(baseValue).WithUpgrade(upgrade));
        return this;
    }


    protected ConstructedCardModel WithSelfDamage(int baseVal, int upgrade = 0)
    {
        WithVar(new DamageVar("SelfDamage", baseVal, ValueProp.Move | ValueProp.Unpowered).WithUpgrade(upgrade));
        return this;
    }

    protected ConstructedCardModel WithEnemyDamage(int baseValue, int upgrade = 0)
    {
        WithVars(new EnemyDamageVar(baseValue, ValueProp.Move).WithUpgrade(upgrade));
        return this;
    }


    protected override void AddExtraArgsToDescription(LocString description)
    {
        foreach (var keyValuePair in _powerCache) description.AddObj(keyValuePair.Key, keyValuePair.Value);
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