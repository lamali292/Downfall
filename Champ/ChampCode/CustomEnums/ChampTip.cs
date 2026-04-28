using BaseLib.Utils;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Champ.ChampCode.CustomEnums; 

public readonly struct ChampTip
{ 
    
    public static readonly ChampTip Finisher = new(nameof(Finisher));

    
    private readonly string _name;

    private ChampTip(string name)
    {
        _name = name;
    }


    public IHoverTip ToHoverTip()
    {
        var key = $"CHAMP-{_name.ToUpperInvariant()}";
        return new HoverTip(
            new LocString("static_hover_tips", $"{key}.title"),
            LocManager.Instance.SmartFormat(
                new LocString("static_hover_tips", $"{key}.description"),
                new Dictionary<string, object> { ["energyPrefix"] = "" }
            )
        );
    }

    public static implicit operator TooltipSource(ChampTip tip)
    {
        return new TooltipSource(_ => tip.ToHoverTip());
    }
}