using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.Code.Abstract;

public abstract class ConstructedPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType stackType = PowerStackType.Counter) : CustomPowerModel
{
    private readonly List<DynamicVar> _dynamicVars = [];
    private readonly List<AbstractTooltipSource<PowerModel>> _hoverTips = [];
    public override PowerType Type => powerType;
    public override PowerStackType StackType => stackType;
    protected sealed override IEnumerable<DynamicVar> CanonicalVars => _dynamicVars;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => _hoverTips.Select(tip => tip.Tip(this));

    protected ConstructedPowerModel WithVars(params DynamicVar[] vars)
    {
        foreach (var dynVar in vars)
        {
            _dynamicVars.Add(dynVar);
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

    protected ConstructedPowerModel WithVar(string name, int baseVal)
    {
        _dynamicVars.Add(new DynamicVar(name, baseVal));
        return this;
    }

    protected ConstructedPowerModel WithBlock(int baseVal)
    {
        _dynamicVars.Add(new BlockVar(baseVal, ValueProp.Move | ValueProp.Unpowered));
        return this;
    }

    protected ConstructedPowerModel WithDamage(int baseVal)
    {
        _dynamicVars.Add(new DamageVar(baseVal, ValueProp.Move | ValueProp.Unpowered));
        return this;
    }


    protected ConstructedPowerModel WithTip(AbstractTooltipSource<PowerModel> tipSource)
    {
        _hoverTips.Add(tipSource);
        return this;
    }

    protected ConstructedPowerModel WithEnergyTip()
    {
        _hoverTips.Add(new PowerTooltipSource(HoverTipFactory.ForEnergy));
        return this;
    }
}