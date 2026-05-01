using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Abstract;

public abstract class CustomStaticTip
{
    private readonly string _name;
    private readonly string _prefix;

    protected CustomStaticTip(string name)
    {
        _prefix = GetType().GetPrefix();
        _name = name;
    }

    public IHoverTip ToHoverTip()
    {
        var key = $"{_prefix}{_name.ToUpperInvariant()}";
        return new HoverTip(
            new LocString("static_hover_tips", $"{key}.title"),
            LocManager.Instance.SmartFormat(
                new LocString("static_hover_tips", $"{key}.description"),
                new Dictionary<string, object> { ["energyPrefix"] = "" }
            )
        );
    }

    public static implicit operator TooltipSource(CustomStaticTip tip)
    {
        return new TooltipSource(_ => tip.ToHoverTip());
    }

    public AbstractTooltipSource<T> AsAbstract<T>() where T : AbstractModel
    {
        return new AbstractTooltipSource<T>(_ => ToHoverTip());
    }

    public static implicit operator AbstractTooltipSource<PowerModel>(CustomStaticTip tip)
    {
        return tip.AsAbstract<PowerModel>();
    }

    public static implicit operator AbstractTooltipSource<RelicModel>(CustomStaticTip tip)
    {
        return tip.AsAbstract<RelicModel>();
    }
}