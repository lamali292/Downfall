using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Downfall.Code.Keywords;

public readonly struct DownfallKeyword
{
    public static readonly DownfallKeyword Scry = new(nameof(Scry));
    public static readonly DownfallKeyword Encode = new(nameof(Encode));
    public static readonly DownfallKeyword Compile = new(nameof(Compile));
    public static readonly DownfallKeyword Cycle = new(nameof(Cycle));
    public static readonly DownfallKeyword Status = new(nameof(Status));
    public static readonly DownfallKeyword Insert = new(nameof(Insert));
    public static readonly DownfallKeyword Conjure = new(nameof(Conjure));
    public static readonly DownfallKeyword Chant = new(nameof(Chant));
    public static readonly DownfallKeyword Drained = new(nameof(Drained));
    private readonly string _name;

    private DownfallKeyword(string name) => _name = name;

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

    public static implicit operator TooltipSource(DownfallKeyword tip)
    {
        return new TooltipSource(_ => tip.ToHoverTip());
    }
}
