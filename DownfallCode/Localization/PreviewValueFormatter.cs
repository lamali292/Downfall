using System.Globalization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace Downfall.DownfallCode.Localization;

public class PreviewValueFormatter : IFormatter
{
    public string Name { get; set; } = "preview";
    public bool CanAutoDetect { get; set; } = false;

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is not DynamicVar var) return false;
        formattingInfo.Write(var.PreviewValue.ToString(CultureInfo.InvariantCulture));
        return true;
    }
}