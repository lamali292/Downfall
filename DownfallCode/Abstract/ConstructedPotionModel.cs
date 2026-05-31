using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Potions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Downfall.DownfallCode.Abstract;

public abstract class ConstructedPotionModel(PotionRarity potionRarity, PotionUsage potionUsage, TargetType targetType)
    : CustomPotionModel
{
    private readonly List<AbstractTooltipSource<PotionModel>> _hoverTips = [];
    private readonly List<Func<PotionModel, IEnumerable<IHoverTip>>> _multiHoverTips = [];

    private readonly List<DynamicVar> _newDynamicVars = [];
    public override PotionRarity Rarity => potionRarity;
    public override PotionUsage Usage => potionUsage;
    public override TargetType TargetType => targetType;
    protected sealed override IEnumerable<DynamicVar> CanonicalVars => _newDynamicVars;

    public sealed override IEnumerable<IHoverTip> ExtraHoverTips => _hoverTips.Select(tip => tip.Tip(this))
        .Concat(_multiHoverTips.SelectMany<Func<PotionModel, IEnumerable<IHoverTip>>, IHoverTip>(mt => mt(this)));


    protected ConstructedPotionModel WithVars(params DynamicVar[] vars)
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

    protected ConstructedPotionModel WithDamage(int i)
    {
        WithVars(new DamageVar(i, ValueProp.Unpowered));
        return this;
    }

    protected ConstructedPotionModel WithCards(int i)
    {
        WithVars(new CardsVar(i));
        return this;
    }


    protected ConstructedPotionModel WithBlock(int i)
    {
        WithTip(StaticHoverTip.Block);
        WithVars(new BlockVar(i, ValueProp.Unpowered));
        return this;
    }

    protected ConstructedPotionModel WithEnergy(int i)
    {
        return WithVars(new EnergyVar(i));
    }

    protected ConstructedPotionModel WithPower<T>(int i, bool showTip = true) where T : PowerModel
    {
        if (showTip)
            _hoverTips.Add(new PotionTooltipSource(_ => HoverTipFactory.FromPower<T>(i)));
        return WithVars(new PowerVar<T>(i));
    }

    protected ConstructedPotionModel WithVar(string name, int baseVal)
    {
        _newDynamicVars.Add(new DynamicVar(name, baseVal));
        return this;
    }

    protected ConstructedPotionModel WithTip(AbstractTooltipSource<PotionModel> tipSource)
    {
        _hoverTips.Add(tipSource);
        return this;
    }

    protected ConstructedPotionModel WithTips(
        Func<PotionModel, IEnumerable<IHoverTip>> multiTipSource)
    {
        _multiHoverTips.Add(multiTipSource);
        return this;
    }


    protected ConstructedPotionModel WithEnergyTip()
    {
        _hoverTips.Add(new PotionTooltipSource(HoverTipFactory.ForEnergy));
        return this;
    }

    public ConstructedPotionModel WithTip<T>() where T : AbstractModel
    {
        return WithTip(typeof(T));
    }
}