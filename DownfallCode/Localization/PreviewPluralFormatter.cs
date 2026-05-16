using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace Downfall.DownfallCode.Localization;

public class PreviewPluralFormatter : IFormatter
{
    public string Name { get; set; } = "pplural";
    public bool CanAutoDetect { get; set; } = false;

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is not DynamicVar var) return false;
        
        var pluralWords = formattingInfo.Format?.Split('|');
        if (pluralWords == null || pluralWords.Count < 2) return false;

        var value = var.PreviewValue;
        var index = value == 1 ? 0 : 1;
        formattingInfo.FormatAsChild(pluralWords[index], formattingInfo.CurrentValue);
        return true;
    }
}