using BaseLib.Extensions;
using BaseLib.Utils;
using Downfall.Code.Config;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using SmartFormat;
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

