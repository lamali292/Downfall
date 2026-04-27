using BaseLib.Abstracts;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Abstract;

public abstract class ConstructedPowerModel(
    PowerType powerType = PowerType.Buff,
    PowerStackType stackType = PowerStackType.Counter) : CustomPowerModel
{
    private readonly List<AbstractTooltipSource<PowerModel>> _hoverTips = [];
    private readonly List<DynamicVar> _newDynamicVars = [];
    public override PowerType Type => powerType;
    public override PowerStackType StackType => stackType;
    protected sealed override IEnumerable<DynamicVar> CanonicalVars => _newDynamicVars;
    protected override IEnumerable<IHoverTip> ExtraHoverTips => _hoverTips.Select(tip => tip.Tip(this));
    public virtual bool ShouldRemoveDueToZero => true;

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

    protected ConstructedPowerModel WithPower<T>(int i) where T : PowerModel
    {
        return WithVars(new PowerVar<T>(i));
    }

    protected ConstructedPowerModel WithVar(string name, int baseVal)
    {
        _newDynamicVars.Add(new DynamicVar(name, baseVal));
        return this;
    }

    protected ConstructedPowerModel WithBlock(int baseVal)
    {
        _newDynamicVars.Add(new BlockVar(baseVal, ValueProp.Move | ValueProp.Unpowered));
        return this;
    }

    protected ConstructedPowerModel WithDamage(int baseVal)
    {
        _newDynamicVars.Add(new DamageVar(baseVal, ValueProp.Move | ValueProp.Unpowered));
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

[HarmonyPatch(nameof(PowerModel), nameof(PowerModel.ShouldRemoveDueToAmount))]
public static class PowerModelMutableClonePatch
{
    [HarmonyPrefix]
    public static bool ShouldRemoveOnZero(PowerModel __instance, ref bool __result)
    {
        if (__instance is not ConstructedPowerModel { ShouldRemoveDueToZero: false }) return true;
        __result = false;
        return false;
    }
}