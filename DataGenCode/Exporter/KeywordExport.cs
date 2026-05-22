using System.Reflection;
using System.Text.Json.Serialization;
using BaseLib.Extensions;
using BaseLib.Patches.Content;
using BaseLib.Utils.Patching;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;

namespace DataGen.DataGenCode.Exporter;

public class KeywordExport : ItemExport, IImageExport
{
    [JsonIgnore] private readonly Texture2D? _icon;

    [JsonInclude] [JsonPropertyName("description")]
    public string Description;

    [JsonInclude] [JsonPropertyName("id")] public string Id;

    [JsonInclude] [JsonPropertyName("name")]
    public string? Title;

    [JsonInclude] [JsonPropertyName("type")]
    public string Type = "Keyword";

    public KeywordExport(HoverTip hoverTip)
    {
        Assembly = hoverTip.GetType().Assembly;
        Title = hoverTip.Title;
        Description = StripBbCodeTags(hoverTip.Description.Replace("98765", "N"), "colorless");
        Id = hoverTip.Id;
        _icon = hoverTip.Icon;
    }

    public KeywordExport(HoverTip hoverTip, object obj) : this(hoverTip, obj.GetType())
    {
    }

    public KeywordExport(HoverTip hoverTip, Type type) : this(hoverTip)
    {
        Assembly = type.Assembly;
    }


    public KeywordExport(CardKeyword cardKeyword) : this((HoverTip)HoverTipFactory.FromKeyword(cardKeyword),
        typeof(CardKeyword))
    {
    }

    public KeywordExport(CardKeyword cardKeyword, Type type) : this((HoverTip)HoverTipFactory.FromKeyword(cardKeyword),
        type)
    {
    }

    public KeywordExport(PowerModel power) : this(power.DumbHoverTip, power)
    {
        _icon = power.BigIcon;
        Type = power.Type == PowerType.None ? "Power" : power.Type.ToString();
    }

    public KeywordExport(OrbModel orb) : this(orb.DumbHoverTip, orb)
    {
        Type = "Orb";
    }


    [JsonInclude]
    [JsonPropertyName("hasIcon")]
    public bool HasIcon => _icon != null;

    public ViewportManager.DrawRequest[] ExportImg(ExportConfig config)
    {
        return HasIcon && _icon != null
            ?
            [
                new ViewportManager.DrawRequest((Vector2I)_icon.GetSize(), $"keywords/{Id}",
                    node => node.DrawTexture(_icon, Vector2.Zero))
            ]
            : [];
    }

    private static KeywordExport? FromDynamicVar(Type type)
    {
        try
        {
            var instance = Activator.CreateInstance(type, 98765m) as DynamicVar;
            if (instance == null) return null;
            var tipFunc = DynamicVarExtensions.DynamicVarTips.Get(instance);
            if (tipFunc == null) return null;
            var tip = (HoverTip)tipFunc(instance);
            return new KeywordExport(tip, type);
        }
        catch
        {
            return null;
        }
    }

    public static List<KeywordExport> FindAll()
    {
        var a = GetCustomEnums.GetEnumsOfType<CardKeyword>()
            .Select(static e => e.GetValue(null) is CardKeyword kw && e.DeclaringType != null
                ? new KeywordExport(kw, e.DeclaringType)
                : null)
            .Where(e => e != null)
            .Cast<KeywordExport>()
            .ToList();
        var b = GetCustomEnums.GetEnumsOfType<StaticHoverTip>()
            .Select(static e => e.GetValue(null) is StaticHoverTip tip && e.DeclaringType != null
                ? new KeywordExport((HoverTip)HoverTipFactory.Static(tip), e.DeclaringType)
                : null)
            .Where(e => e != null)
            .Cast<KeywordExport>()
            .ToList();
        var c = ModManager.Mods.SelectMany(static m => m.assembly?.GetTypes().Where(t =>
                    t.IsAssignableTo(typeof(DynamicVar)) && t.GetConstructors().Any(c =>
                        c.GetParameters() is { Length: 1 } parameters &&
                        parameters[0].ParameterType == typeof(decimal))).Select(static v => FromDynamicVar(v)) ?? []
            )
            .Where(e => e != null)
            .Cast<KeywordExport>()
            .ToList();
        return
        [
            ..Enum.GetValues<CardKeyword>().Select(static e => new KeywordExport(e)),
            ..a,
            ..Enum.GetValues<StaticHoverTip>().Where(static e => !e.ToString().EndsWith("Dynamic"))
                .Select(static e => new KeywordExport((HoverTip)HoverTipFactory.Static(e))),
            ..b,
            ..ModelDb.AllPowers.Select(static p => new KeywordExport(p)),
            ..OrbModel._validOrbs.Select(static o => new KeywordExport(ModelDb.GetById<OrbModel>(o))),
            ..c
        ];
    }
}

[HarmonyPatch("GenEnumValues", "FindAndGenerate")]
public class GetCustomEnums
{
    private static readonly Dictionary<Type, List<FieldInfo>> Enums = [];

    public static List<FieldInfo> GetEnumsOfType<T>()
    {
        return Enums.TryGetValue(typeof(T), out var list) ? list : [];
    }

    private static void Store(FieldInfo field, object key)
    {
        if (field.DeclaringType == null) return;
        if (!Enums.TryGetValue(field.FieldType, out var list))
        {
            list = [];
            Enums[field.FieldType] = list;
        }

        list.Add(field);
    }

    private static List<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        return new InstructionPatcher(instructions)
            .Match(new InstructionMatcher()
                .call(AccessTools.Method(typeof(CustomEnums), nameof(CustomEnums.GenerateKey), [typeof(FieldInfo)]))
            )
            .Step()
            .Insert([
                CodeInstruction.LoadLocal(8),
                CodeInstruction.LoadLocal(11),
                CodeInstruction.Call(typeof(GetCustomEnums), nameof(Store))
            ]);
    }
}