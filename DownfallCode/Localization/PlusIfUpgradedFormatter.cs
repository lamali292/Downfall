using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace Downfall.DownfallCode.Localization;

public class PlusIfUpgradedFormatter : IFormatter
{
    public string Name
    {
        get => "plus";
        set => throw new NotSupportedException();
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is not IfUpgradedVar currentValue)
            return false;

        switch (currentValue.upgradeDisplay)
        {
            case UpgradeDisplay.Normal:
                break;
            case UpgradeDisplay.Upgraded:
                formattingInfo.Write("+");
                break;
            case UpgradeDisplay.UpgradePreview:
                formattingInfo.Write("[green]+[/green]");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(formattingInfo),
                    $"Unexpected value: {currentValue.upgradeDisplay}");
        }

        return true;
    }
}