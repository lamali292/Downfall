using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Collector.CollectorCode.CustomEnums;

public class CollectorTip
{
    [CustomEnum] public static StaticHoverTip Kindle;
    [CustomEnum] public static StaticHoverTip Pyred;
    
    public static HoverTip ReserveTip => new(
        new LocString("static_hover_tips", "COLLECTOR-RESERVE.title"),
        new LocString("static_hover_tips", "COLLECTOR-RESERVE.description"),
        PreloadManager.Cache.GetTexture2D("res://Collector/images/character/reserve_icon.png"));
}