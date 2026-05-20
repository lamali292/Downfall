using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Abstract;

public abstract class ConstructedRelicModel(RelicRarity rarity) : CustomRelicModel
{
    private readonly List<AbstractTooltipSource<RelicModel>> _hoverTips = [];
    private readonly List<Func<RelicModel, IEnumerable<IHoverTip>>> _multiHoverTips = [];

    private readonly List<DynamicVar> _newDynamicVars = [];
    protected sealed override IEnumerable<DynamicVar> CanonicalVars => _newDynamicVars;
    protected sealed override IEnumerable<IHoverTip> ExtraHoverTips => _hoverTips.Select(tip => tip.Tip(this))
        .Concat(_multiHoverTips.SelectMany<Func<RelicModel, IEnumerable<IHoverTip>>, IHoverTip>(mt => mt(this)));
    public override RelicRarity Rarity => rarity;

    protected ConstructedRelicModel WithVars(params DynamicVar[] vars)
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

    
    protected ConstructedRelicModel WithCards(int i)
    {
        WithVars(new CardsVar(i));
        return this;
    }

    
    protected ConstructedRelicModel WithBlock(int i)
    {
        WithTip(StaticHoverTip.Block);
        WithVars(new BlockVar(i, ValueProp.Unpowered));
        return this;
    }

    protected ConstructedRelicModel WithEnergy(int i)
    {
        return WithVars(new EnergyVar(i));
    }

    protected ConstructedRelicModel WithPower<T>(int i) where T : PowerModel
    {
        return WithVars(new PowerVar<T>(i));
    }

    protected ConstructedRelicModel WithVar(string name, int baseVal)
    {
        _newDynamicVars.Add(new DynamicVar(name, baseVal));
        return this;
    }

    protected ConstructedRelicModel WithTip(AbstractTooltipSource<RelicModel> tipSource)
    {
        _hoverTips.Add(tipSource);
        return this;
    }
    
    protected ConstructedRelicModel WithTips(
        Func<RelicModel, IEnumerable<IHoverTip>> multiTipSource)
    {
        this._multiHoverTips.Add(multiTipSource);
        return this;
    }


    protected ConstructedRelicModel WithEnergyTip()
    {
        _hoverTips.Add(new RelicTooltipSource(HoverTipFactory.ForEnergy));
        return this;
    }
}