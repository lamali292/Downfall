using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace Collector.CollectorCode.Localization;

public class ReserveIconsFormatter : IAutoRegisterFormatSpecifier
{

    public string Name
    {
        get => "reserveIcons";
        set => throw new Exception();
    }

    public bool CanAutoDetect { get; set; }
    
    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        var result = formattingInfo.CurrentValue switch
        {
            CalculatedVar calculatedVar => Convert.ToInt32(calculatedVar.Calculate(null)),
            DynamicVar dynamicVar => Convert.ToInt32(dynamicVar.PreviewValue),
            decimal num1 => (int)num1,
            int num2 => num2,
            _ =>  int.TryParse(formattingInfo.FormatterOptions, out var str) ? str : 1
        
        };

        const string element = "[img]res://Collector/images/character/reserve_text_icon.png[/img]";
        string text;
        if (result is > 0 and < 4)
            text = string.Concat(Enumerable.Repeat<string>(element, result));
        else if (formattingInfo.CurrentValue is DynamicVar currentValue)
            text = currentValue.ToHighlightedString(false) + element;
        else
            text = $"{result}{element}";
        formattingInfo.Write(text);
        return true;
    }

}