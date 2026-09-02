using BaseLib.Config;
using Downfall.DownfallCode.Patches.KaleidoscopePatch;

namespace Downfall.DownfallCode.Config;

[ConfigHoverTipsByDefault]
public class DownfallConfig : SimpleModConfig
{
    [ConfigSection("Sync")] 
    public static PrismaticMode PrismaticOption { get; set; } = PrismaticMode.All;
    
    
    [ConfigSection("HideSection")]
    public static bool HideAutomaton { get; set; } = false;
    public static bool HideAwakened { get; set; } = false;
    public static bool HideChamp { get; set; } = false;
    public static bool HideCollector { get; set; } = false;
    //public static bool HideGremlins { get; set; } = false;
    public static bool HideGuardian { get; set; } = false;
    public static bool HideHermit { get; set; } = false;
    public static bool HideHexaghost { get; set; } = false;
    public static bool HideSlimeboss { get; set; } = false;
    public static bool HideSnecko { get; set; } = false;
    
    [ConfigSection("Dev")]
    public static bool DevMode { get; set; } = false;
}