using System.Reflection;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using MegaCrit.Sts2.Core.Modding;
using FileAccess = Godot.FileAccess;

namespace DataGen.DataGenCode.Exporter;

public class ModExport
{
    [JsonInclude] [JsonPropertyName("authors")]
    private readonly string[] _authors = [];

    [JsonInclude] [JsonPropertyName("description")]
    private readonly string? _description = "";

    [JsonInclude] [JsonPropertyName("version")]
    private readonly string? _version = "";

    [JsonIgnore] public readonly Assembly? Assembly;

    [JsonInclude] [JsonPropertyName("id")] public readonly string? Id;

    [JsonIgnore] public readonly bool IsBasegame = false;

    public readonly ItemList Items;

    [JsonInclude] [JsonPropertyName("name")]
    public readonly string? Name;

    private ModExport()
    {
        Items = new ItemList(this);
    }

    public ModExport(Mod mod) : this()
    {
        Id = mod.manifest?.id;
        Name = mod.manifest?.name;
        _version = mod.manifest?.version;
        var author = mod.manifest?.author;
        _authors = author == null ? [] : [author];
        _description = mod.manifest?.description;

        Assembly = mod.assembly;
    }

    [JsonInclude]
    [JsonPropertyName("credits")]
    private string Credits { get; } = "";

    [JsonInclude]
    [JsonPropertyName("stsVersion")]
    private byte SlayTheSpireVersion { get; } = 2;

    public void AddItem(ItemExport item)
    {
        item.Mod = this;
        Items.Add(item);
    }

    public void Export(string basePath)
    {
        var dir = $"{basePath}/{Id}";
        DirAccess.MakeDirRecursiveAbsolute(dir);
        var file = FileAccess.Open($"{dir}/items.json", FileAccess.ModeFlags.Write);
        file.StoreString(JsonSerializer.Serialize(Items,
            new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        file.Close();
    }
}