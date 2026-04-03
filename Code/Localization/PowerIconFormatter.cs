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


public class PowerIconFormatter : IFormatter
{
    public string Name
    {
        get => "icon";
        set => throw new NotImplementedException();
    }

    public bool CanAutoDetect { get; set; }

    public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is PowerModel power)
        {
            string text;
            if (DownfallConfig.IconPowers)
            {
                var iconPath = $"res://Downfall/images/atlases/power_sprite_atlas.sprites/{power.Id.Entry.RemovePrefix().ToLowerInvariant()}.tres";
                text = $"[img]{iconPath}[/img]";
            }
            else
            {
                text = $"[gold]{power.Title.GetRawText()}[/gold]";
            }
            formattingInfo.Write(text);
        }
        else
        {
            var options = formattingInfo.FormatterOptions;
            var optionParts = options.Split(',', 2);
            var powerName = optionParts[0].Trim();
            var defaultText = optionParts.Length == 2 ? optionParts[1].Trim() : null;

            var parts = powerName.Split('-', 2);
            var id = parts.Length == 2 ?
                $"{parts[0].ToUpperInvariant()}-{parts[1].ToSnakeCase().ToUpperInvariant()}" :
                powerName.ToSnakeCase().ToUpperInvariant();

            if (string.IsNullOrEmpty(id)) return false;

            string text;
            if (DownfallConfig.IconPowers)
            {
                var iconPath = $"res://Downfall/images/atlases/power_sprite_atlas.sprites/{id.RemovePrefix().ToLowerInvariant()}.tres";
                text = $"[img]{iconPath}[/img]";
            }
            else if (defaultText != null)
            {
                text = $"[gold]{defaultText}[/gold]";
            }
            else
            {
                var power2 = ModelDb.GetByIdOrNull<PowerModel>(new ModelId("POWER", id));
                if (power2 == null) return false;
                text = $"[gold]{power2.Title.GetRawText()}[/gold]";
            }
            formattingInfo.Write(text);
        }
    
        
        return true;
    }
}

[HarmonyPatch(typeof(LocManager), nameof(LocManager.Initialize))]
public static class LocManagerPatch
{
    [HarmonyPostfix]
    private static void AddCustomFormatters()
    {
        Smart.Default.AddExtensions(new PowerIconFormatter());
    }
}