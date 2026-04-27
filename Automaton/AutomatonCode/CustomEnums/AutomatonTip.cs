using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Automaton.AutomatonCode.CustomEnums;

public readonly struct  AutomatonTip
{
    public static readonly AutomatonTip Encode = new(nameof(Encode));
    public static readonly AutomatonTip Compile = new(nameof(Compile));
    public static readonly AutomatonTip Cycle = new(nameof(Cycle));
    public static readonly AutomatonTip Status = new(nameof(Status));
    public static readonly AutomatonTip Insert = new(nameof(Insert));
    
    
    private readonly string _name;

    private AutomatonTip(string name)
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

    public static implicit operator TooltipSource(AutomatonTip tip)
    {
        return new TooltipSource(_ => tip.ToHoverTip());
    }
}