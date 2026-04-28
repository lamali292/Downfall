using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Awakened.AwakenedCode.CustomEnums;

public readonly struct AwakenedTip
{

    public static readonly AwakenedTip Conjure = new(nameof(Conjure));
    public static readonly AwakenedTip Chant = new(nameof(Chant));
    public static readonly AwakenedTip Drained = new(nameof(Drained));
    
    private readonly string _name;

    private AwakenedTip(string name)
    {
        _name = name;
    }


    public IHoverTip ToHoverTip()
    {
        var key = $"AWAKENED-{_name.ToUpperInvariant()}";
        return new HoverTip(
            new LocString("static_hover_tips", $"{key}.title"),
            LocManager.Instance.SmartFormat(
                new LocString("static_hover_tips", $"{key}.description"),
                new Dictionary<string, object> { ["energyPrefix"] = "" }
            )
        );
    }

    public static implicit operator TooltipSource(AwakenedTip tip)
    {
        return new TooltipSource(_ => tip.ToHoverTip());
    }
}