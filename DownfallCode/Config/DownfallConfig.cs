using BaseLib.Config;
using Downfall.DownfallCode.Patches.KaleidoscopePatch;

namespace Downfall.DownfallCode.Config;

[ConfigHoverTipsByDefault]
public class DownfallConfig : SimpleModConfig
{
    [ConfigSection("UI")]
    public static bool ShowArtSubmissionButton { get; set; } = true;
    
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

    public static bool IsDisabled(string character)
    {
        return character switch
        {
            "Automaton" => HideAutomaton,
            "Awakened" => HideAwakened,
            "Champ" => HideChamp,
            "Collector" => HideCollector,
            "Guardian" => HideGuardian,
            "Hermit" => HideHermit,
            "Hexaghost" => HideHexaghost,
            "SlimeBoss" => HideSlimeboss,
            "Snecko" => HideSnecko,
            _ => false
        };
    }

    public static void SetDisabled(string character, bool value)
    {
        switch (character)
        {
            case "Automaton": HideAutomaton = value; break;
            case "Awakened": HideAwakened = value; break;
            case "Champ": HideChamp = value; break;
            case "Collector": HideCollector = value; break;
            case "Guardian": HideGuardian = value; break;
            case "Hermit": HideHermit = value; break;
            case "Hexaghost": HideHexaghost = value; break;
            case "SlimeBoss": HideSlimeboss = value; break;
            case "Snecko": HideSnecko = value; break;
        }
    }
}
