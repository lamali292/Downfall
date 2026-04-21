using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Downfall.Code.Keywords;

public readonly struct DownfallTip
{
    public static readonly DownfallTip Scry = new(nameof(Scry));
    public static readonly DownfallTip Encode = new(nameof(Encode));
    public static readonly DownfallTip Compile = new(nameof(Compile));
    public static readonly DownfallTip Cycle = new(nameof(Cycle));
    public static readonly DownfallTip Status = new(nameof(Status));
    public static readonly DownfallTip Insert = new(nameof(Insert));
    public static readonly DownfallTip Conjure = new(nameof(Conjure));
    public static readonly DownfallTip Chant = new(nameof(Chant));
    public static readonly DownfallTip Drained = new(nameof(Drained));
    public static readonly DownfallTip Finisher = new(nameof(Finisher));
    public static readonly DownfallTip Kindle = new(nameof(Kindle));
    public static readonly DownfallTip Accelerate = new(nameof(Accelerate));
    public static readonly DownfallTip Stasis = new(nameof(Stasis));
    public static readonly DownfallTip Brace = new(nameof(Brace));
    public static readonly DownfallTip Tick = new(nameof(Tick));
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