using BaseLib.Config;
using BaseLib.Config.UI;
using BaseLib.Extensions;
using Downfall.DownfallCode.Config.UI;
using Godot;
using Downfall.DownfallCode.Patches.KaleidoscopePatch;

namespace Downfall.DownfallCode.Config;

[ConfigHoverTipsByDefault]
public class DownfallConfig : SimpleModConfig
{
    [ConfigSection("UI")]
    public static bool ShowArtSubmissionButton { get; set; } = true;

    [ConfigColorPicker(EditAlpha = false)]
    public static Color AfterlifeColor { get; set; } = new("#78D1A0");

    [ConfigSection("Sync")]
    public static PrismaticMode PrismaticOption { get; set; } = PrismaticMode.All;

    public override void SetupConfigUI(Control optionContainer)
    {
        GenerateOptionsForAllProperties(optionContainer);
        AddResetButtonToColorRow(optionContainer, nameof(AfterlifeColor));
        AddRestoreDefaultsButton(optionContainer);
        SetupFocusNeighbors(optionContainer);
    }

    // Swaps the row's plain swatch for one with a reset-to-default button; the row keeps its label, hover tip and section.
    private void AddResetButtonToColorRow(Control optionContainer, string propertyName)
    {
        var row = optionContainer.GetNodeOrNull<NConfigOptionRow>($"%{propertyName}");
        if (row == null) return;

        var picker = new NResettableColorPicker(this, GetType().GetProperty(propertyName)!, GetLabelText("ResetColor"));
        row.SettingControl.QueueFree();
        row.RemoveChild(row.SettingControl);
        row.AddChild(picker);
        picker.ClearFocusNeighbors();
    }
    
    
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

    
    [ConfigSection("Internal")]
    public static bool EnableCompendiumTools { get; set; } = false;

    
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
