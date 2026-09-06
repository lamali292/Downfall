using BaseLib.Patches.Content;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.HoverTips;

namespace Collector.CollectorCode.CustomEnums;

public class CollectorTip
{
    [CustomEnum] public static StaticHoverTip Kindle;
    [CustomEnum] public static StaticHoverTip Pyred;
    
    public static HoverTip ReserveTip => new(
        HoverTipFactory.L10NStatic("COLLECTOR-RESERVE.title"),
        HoverTipFactory.L10NStatic("COLLECTOR-RESERVE.description"),
        PreloadManager.Cache.GetTexture2D("res://Collector/images/character/reserve_icon.png"));
}