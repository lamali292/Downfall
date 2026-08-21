using System.Globalization;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace Downfall.DownfallCode.Localization;

public class OrdinalFormatter : IAutoRegisterFormatSpecifier 
{
    public string Name
    {
        get => "ordinal";
        set => throw new NotSupportedException();
    }
    
    public bool CanAutoDetect { get; set; } = false;
 
    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is not (int or long or DynamicVar))
            return false;
 
        var n = Convert.ToInt64(formattingInfo.CurrentValue);
        
        var gender = 'm';
        var opt = formattingInfo.FormatterOptions;
        if (!string.IsNullOrEmpty(opt))
            gender = char.ToLowerInvariant(opt[0]);
 
        formattingInfo.Write(ToOrdinal(n, Culture.TwoLetterISOLanguageName, gender));
        return true;
    }
 
    private static string ToOrdinal(long n, string lang, char g) => lang switch
    {
        "ja" or "zh" => "第" + n,
        "th" => "ที่ " + n,    
        "de" or "pl" or "tr" => n + ".",
        "fr" => n == 1 ? (g == 'f' ? "1re" : "1er") : n + "e",
        "es" => n + (g == 'f' ? ".ª" : ".º"),
        "it" or "pt" => n + (g == 'f' ? "ª" : "º"),
        "ru" => n + g switch
        {
            'f' => "-я",
            'n' => "-е",
            _ => "-й"
        },
        "ko" => n + "번째",
        _ => ToEnglishOrdinal(n),
    };
 
    private static string ToEnglishOrdinal(long n)
    {
        var abs = Math.Abs(n);
        if (abs % 100 is >= 11 and <= 13)
            return n + "th";
        return (abs % 10) switch
        {
            1 => n + "st",
            2 => n + "nd",
            3 => n + "rd",
            _ => n + "th",
        };
    }

    private static CultureInfo Culture => LocManager.Instance.CultureInfo;
}
