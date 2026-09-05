using BaseLib.Abstracts;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Abstract;

public abstract class ConstructedPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType stackType = PowerStackType.Counter) : CustomPowerModel, IHasSecondAmount
{
    private readonly List<AbstractTooltipSource<PowerModel>> _hoverTips = [];
    private readonly List<Func<PowerModel, IEnumerable<IHoverTip>>> _multiHoverTips = [];
    protected virtual int? SecondAmount => null;
    private readonly List<DynamicVar> _newDynamicVars = [];
    public override PowerType Type => powerType;
    public override PowerStackType StackType => stackType;
    protected sealed override IEnumerable<DynamicVar> CanonicalVars => _newDynamicVars;

    protected sealed override IEnumerable<IHoverTip> ExtraHoverTips => _hoverTips.Select(tip => tip.Tip(this))
        .Concat(_multiHoverTips.SelectMany(e => e.Invoke(this)));

    public virtual bool ShouldRemoveDueToZero => true;


    protected ConstructedPowerModel WithUpgradedCardTip<T>(Action<T, PowerModel>? modifyTipCard = null)
        where T : CardModel
    {
        return WithTip(new PowerTooltipSource(power =>
        {
            var mutable = ModelDb.Card<T>().ToMutable();
            mutable.UpgradeInternal();
            if (mutable is T obj2) modifyTipCard?.Invoke(obj2, power);
            return HoverTipFactory.FromCard(mutable);
        }));
    }

    protected ConstructedPowerModel WithCardTip<T>(Action<T, PowerModel>? modifyTipCard = null)
        where T : CardModel
    {
        return WithTip(new PowerTooltipSource(power =>
        {
            var mutable = ModelDb.Card<T>().ToMutable();
            if (mutable is T obj2) modifyTipCard?.Invoke(obj2, power);
            return HoverTipFactory.FromCard(mutable);
        }));
    }

    protected ConstructedPowerModel WithVars(params DynamicVar[] vars)
    {
        foreach (var dynVar in vars)
        {
            _newDynamicVars.Add(dynVar);
            var type = dynVar.GetType();
            if (!type.IsGenericType) continue;

            foreach (var arg in type.GetGenericArguments())
            {
                if (!arg.IsAssignableTo(typeof(PowerModel))) continue;
                WithTip(arg);
            }
        }

        return this;
    }

    protected ConstructedPowerModel WithPower<T>(decimal i) where T : PowerModel
    {
        return WithVars(new PowerVar<T>(i));
    }

    protected ConstructedPowerModel WithVar(string name, decimal baseVal)
    {
        _newDynamicVars.Add(new DynamicVar(name, baseVal));
        return this;
    }

    protected ConstructedPowerModel WithBlock(decimal baseVal)
    {
        _newDynamicVars.Add(new BlockVar(baseVal, BlockProps.nonCardUnpowered));
        return this;
    }

    protected ConstructedPowerModel WithCards(int baseVal)
    {
        _newDynamicVars.Add(new CardsVar(baseVal));
        return this;
    }

    public ConstructedPowerModel WithEnergy(int baseVal)
    {
        _newDynamicVars.Add(new EnergyVar(baseVal));
        WithEnergyTip();
        return this;
    }


    protected ConstructedPowerModel WithDamage(decimal baseVal)
    {
        _newDynamicVars.Add(new DamageVar(baseVal, DamageProps.nonCardUnpowered));
        return this;
    }


    protected ConstructedPowerModel WithTip(AbstractTooltipSource<PowerModel> tipSource)
    {
        _hoverTips.Add(tipSource);
        return this;
    }

    protected ConstructedPowerModel WithTips(
        Func<PowerModel, IEnumerable<IHoverTip>> multiTipSource)
    {
        _multiHoverTips.Add(multiTipSource);
        return this;
    }

    protected ConstructedPowerModel WithEnergyTip()
    {
        _hoverTips.Add(new PowerTooltipSource(HoverTipFactory.ForEnergy));
        return this;
    }


    public ConstructedPowerModel WithTip<T>() where T : AbstractModel
    {
        return WithTip(typeof(T));
    }


    public static IEnumerable<DynamicVar> FinishMakeCalculatedVar(
        CalculatedVar var,
        int baseVal,
        int bonusVal)
    {
        switch (var)
        {
            case CustomCalculatedVar _:
            case CustomCalculatedBlockVar _:
                yield return new DynamicVar(var.Name + "Base", baseVal);
                yield return new DynamicVar(var.Name + "Extra", bonusVal);
                break;
            case CustomCalculatedDamageVar _:
                yield return new DynamicVar(var.Name + "Base", baseVal);
                yield return new CustomExtraDamageVar(var.Name, bonusVal);
                break;
            case CalculatedDamageVar _:
                yield return new CalculationBaseVar(baseVal);
                yield return new ExtraDamageVar(bonusVal);
                break;
            default:
                yield return new CalculationBaseVar(baseVal);
                yield return new CalculationExtraVar(bonusVal);
                break;
        }

        yield return var;
    }


    public static IEnumerable<DynamicVar> MakeCalculatedVar(
        string name,
        int baseVal,
        Func<PowerModel, Creature?, Decimal> bonus,
        int mult = 1)
    {
        return CustomCardModel.FinishMakeCalculatedVar(
            new CustomCalculatedVar(name).WithMultiplier(bonus), baseVal, mult);
    }

    public static IEnumerable<DynamicVar> MakeCalculatedDamage(
        string name,
        int baseVal,
        Func<PowerModel, Creature?, Decimal> bonus,
        int mult = 1,
        ValueProp props = ValueProp.Move)
    {
        return CustomCardModel.FinishMakeCalculatedVar(
            new CustomCalculatedDamageVar(name, props).WithMultiplier(bonus), baseVal, mult);
    }

    public static IEnumerable<DynamicVar> MakeCalculatedBlock(
        string name,
        int baseVal,
        Func<PowerModel, Creature?, Decimal> bonus,
        int mult = 1,
        ValueProp props = ValueProp.Move)
    {
        return CustomCardModel.FinishMakeCalculatedVar(
            new CustomCalculatedBlockVar(name, props).WithMultiplier(bonus), baseVal, mult);
    }

    public string GetSecondAmount() => SecondAmount.ToString() ?? "";
}