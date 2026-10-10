using BaseLib.Extensions;
using BaseLib.Cards.Variables;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.DynamicVars;

/// <summary>
/// <see cref="CustomCalculatedDamageVar"/> for owners that are <see cref="ICustomAbstractModel"/>s
/// (e.g. SlimeBoss slimes), which BaseLib's owner switch doesn't know. Like the BaseLib version it
/// needs "{Name}Base" and "{Name}Extra" vars in the same set - use <see cref="Create"/>.
/// Value = Base + Extra * multiplier(owner), re-evaluated on every read.
/// </summary>
public class CustomModelCalculatedDamageVar(string name, ValueProp props) : CustomCalculatedDamageVar(name, props)
{
    private Func<ICustomAbstractModel, Creature?, decimal>? _modelCalc;

    /// <summary>The Base, Extra and calculated vars for <c>baseVal + extra * multiplier(model)</c>.</summary>
    public static IEnumerable<DynamicVar> Create(string name, ValueProp props, int baseVal,
        Func<ICustomAbstractModel, Creature?, decimal> multiplier, int extra = 1)
    {
        var calculated = new CustomModelCalculatedDamageVar(name, props).WithModelMultiplier(multiplier);
        yield return new DynamicVar($"{name}Base", baseVal);
        yield return new DynamicVar($"{name}Extra", extra);
        yield return calculated;
    }

    public CustomModelCalculatedDamageVar WithModelMultiplier(Func<ICustomAbstractModel, Creature?, decimal> multiplier)
    {
        if (_modelCalc != null)
            throw new InvalidOperationException($"Tried to set model multiplier calc on {Name} twice!");
        if (multiplier.Target is MegaCrit.Sts2.Core.Models.AbstractModel)
            throw new InvalidOperationException("Multiplier calc must be static!");
        _modelCalc = multiplier;
        return this;
    }

    public override decimal CalculateCustom(Creature? target)
    {
        if (_owner is not ICustomAbstractModel model)
            return base.CalculateCustom(target);
        // Canonical instances aren't in a combat; mirror the game's "no multiplier outside combat".
        var mult = _owner.IsMutable ? _modelCalc?.Invoke(model, target) ?? 0m : 0m;
        return GetBaseVar().BaseValue + GetExtraVar().BaseValue * mult;
    }

    protected override DynamicVar GetBaseVar() => Lookup($"{Name}Base");

    protected override DynamicVar GetExtraVar() => Lookup($"{Name}Extra");

    private DynamicVar Lookup(string key) =>
        _owner is ICustomAbstractModel model ? model.DynamicVars[key] : _owner!.GetDynamicVar(key);
}
