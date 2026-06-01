using System.Globalization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;
using SmartFormat.Core.Formatting;
using SmartFormat.Utilities;

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
        var culture = GetCultureInfo(formattingInfo);
        var pluralRule = PluralRules.GetPluralRule(culture.TwoLetterISOLanguageName);
        var index = pluralRule(var.PreviewValue, pluralWords.Count);
        formattingInfo.FormatAsChild(pluralWords[index], formattingInfo.CurrentValue);
        return true;
    }

    private static CultureInfo GetCultureInfo(IFormattingInfo formattingInfo)
    {
        var culture = formattingInfo.FormatterOptions.Trim();
        CultureInfo cultureInfo;
        if (culture == string.Empty)
        {
            if (formattingInfo.FormatDetails.Provider is CultureInfo ci)
                cultureInfo = ci;
            else
                cultureInfo = CultureInfo.CurrentUICulture;
            if (cultureInfo.Equals(CultureInfo.InvariantCulture))
                cultureInfo = CultureInfo.GetCultureInfo("en");
        }
        else
        {
            try
            {
                cultureInfo = CultureInfo.GetCultureInfo(culture);
            }
            catch (Exception e)
            {
                throw new FormattingException(formattingInfo.Format, e, 0);
            }
        }

        return cultureInfo;
    }
}