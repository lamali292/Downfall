using SmartFormat.Core.Extensions;

namespace Downfall.Code.Localization;

public class FinisherFormatter : IFormatter
{
    public string Name
    {
        get => "finisher";
        set => throw new NotImplementedException();
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        formattingInfo.Write("[img]res://Downfall/images/ui/finisher.png[/img]");
        return true;
    }
}