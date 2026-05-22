using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace DataGen.DataGenCode.Exporter;

public abstract partial class ItemExport
{
    private static readonly Regex BbCodeSubstitutor = BbCodeRegex();

    [JsonInclude] [JsonPropertyName("v")] private byte SlayTheSpireVersion { get; } = 2;

    [JsonIgnore] public Assembly? Assembly;

    [JsonInclude] [JsonPropertyName("mod")] [JsonConverter(typeof(ModPropertyConverter))]
    public ModExport? Mod;

    [GeneratedRegex(@"\[.*?\]")]
    private static partial Regex BbCodeRegex();
    //protected static string StripBBCodeTags(string s) => Regex.Unescape(BBCodeSubstitutor.Replace(BBCodeImgSubstitutor.Replace(s, static m =>{
    //    var path = m.Value["[img]".Length..(m.Value.Length-"[/img]".Length)];
    //    return $"{{img={path[(path.LastIndexOf('/')+1)..path.LastIndexOf('.')]}}}";
    //}), static m => ""));

    protected static string StripBbCodeTags(string s, AbstractModel model)
    {
        if (EnergyIconHelper.GetPool(model) is ICustomEnergyIconPool { TextEnergyIconPath: not null } pool) return StripBbCodeTags(s, true, pool.TextEnergyIconPath);
        return StripBbCodeTags(s, EnergyIconHelper.GetPrefix(model));
    }

    protected static string StripBbCodeTags(string s, string prefix)
    {
        return StripBbCodeTags(s, true, $"res://images/packed/sprite_fonts/{prefix}_energy_icon.png");
    }

    protected static string StripBbCodeTags(string s, bool fullPath, string iconPath)
    {
        return BbCodeSubstitutor.Replace(
            s
                .Replace($"[img]{iconPath}[/img]", "{E}")
                .Replace("[img]res://images/packed/sprite_fonts/star_icon.png[/img]", "{STAR}"),
            static _ => ""
        ).Replace("{E}", "[E]").Replace("{STAR}", "[STAR]");
    }

    private class ModPropertyConverter : JsonConverter<ModExport>
    {
        public override ModExport Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ModExport value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Name);
        }
    }
}

public interface IImageExport
{
    public ViewportManager.DrawRequest[] ExportImg(ExportConfig config);
}