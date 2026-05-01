using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Downfall.DownfallCode.CustomEnums;

public readonly struct DownfallTip
{
    public static readonly DownfallTip Scry = new(nameof(Scry));
    public static readonly DownfallTip Status = new(nameof(Status));

    private readonly string _name;

    private DownfallTip(string name)
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

    public static implicit operator TooltipSource(DownfallTip tip)
    {
        return new TooltipSource(_ => tip.ToHoverTip());
    }
}