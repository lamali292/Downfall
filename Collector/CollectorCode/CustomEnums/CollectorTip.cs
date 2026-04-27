using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Collector.CollectorCode.CustomEnums;


public readonly struct CollectorTip
{
    public static readonly CollectorTip Kindle = new(nameof(Kindle));
    
    private readonly string _name;

    private CollectorTip(string name)
    {
        _name = name;
    }


    public IHoverTip ToHoverTip()
    {
        var key = $"DOWNFALL-{_name.ToUpperInvariant()}";
        return new HoverTip(
            new LocString("static_hover_tips", $"{key}.title"),
            LocManager.Instance.SmartFormat(
                new LocString("static_hover_tips", $"{key}.description"),
                new Dictionary<string, object> { ["energyPrefix"] = "" }
            )
        );
    }

    public static implicit operator TooltipSource(CollectorTip tip)
    {
        return new TooltipSource(_ => tip.ToHoverTip());
    }
}