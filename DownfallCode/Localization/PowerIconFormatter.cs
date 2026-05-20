using Downfall.DownfallCode.Abstract;
using SmartFormat.Core.Extensions;

namespace Downfall.DownfallCode.Localization;

public class PowerIconFormatter : IFormatter
{
    public string Name
    {
        get => "icon";
        set => throw new Exception();
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is not DownfallPowerModel power) return false;
        formattingInfo.Write($"[img]{power.CustomPackedSpritePath}[/img]");
        return true;
    }
}