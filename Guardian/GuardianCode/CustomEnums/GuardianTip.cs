using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Guardian.GuardianCode.CustomEnums;


public readonly struct GuardianTip
{
    public static readonly GuardianTip Accelerate = new(nameof(Accelerate));
    public static readonly GuardianTip Stasis = new(nameof(Stasis));
    public static readonly GuardianTip Brace = new(nameof(Brace));
    public static readonly GuardianTip Tick = new(nameof(Tick));
    
    
    private readonly string _name;

    private GuardianTip(string name)
    {
        _name = name;
    }


    public IHoverTip ToHoverTip()
    {
        var key = $"{GuardianMainFile.ModId.ToUpperInvariant()}-{_name.ToUpperInvariant()}";
        return new HoverTip(
            new LocString("static_hover_tips", $"{key}.title"),
            LocManager.Instance.SmartFormat(
                new LocString("static_hover_tips", $"{key}.description"),
                new Dictionary<string, object> { ["energyPrefix"] = "" }
            )
        );
    }

    public static implicit operator TooltipSource(GuardianTip tip)
    {
        return new TooltipSource(_ => tip.ToHoverTip());
    }
}