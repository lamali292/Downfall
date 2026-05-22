using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;
using MegaCrit.Sts2.Core.Modding;
using FileAccess = Godot.FileAccess;

namespace DataGen.DataGenCode.Exporter;

public class ModExport {
    [JsonInclude][JsonPropertyName("id")]
    public readonly string? Id;
    [JsonInclude][JsonPropertyName("name")]
    public readonly string? Name;
    [JsonInclude][JsonPropertyName("version")]
    private readonly string? _version = "";
    [JsonInclude][JsonPropertyName("authors")]
    private readonly string[] _authors = [];
    [JsonInclude][JsonPropertyName("credits")]
    private string Credits { get; } = "";
    [JsonInclude][JsonPropertyName("description")]
    private readonly string? _description = "";
    [JsonInclude][JsonPropertyName("stsVersion")]
    private byte SlayTheSpireVersion { get; } = 2;

    [JsonIgnore]
    public readonly bool IsBasegame = false;
    [JsonIgnore]
    public readonly Assembly? Assembly;

    private ModExport() {
        Items = new ItemList(this);
    }

    public ModExport(Mod mod) : this() {
        Id = mod.manifest?.id;
        Name = mod.manifest?.name;
        _version = mod.manifest?.version;
        var author = mod.manifest?.author;
        _authors = author == null ? [] : [author];
        _description = mod.manifest?.description;
        
        Assembly = mod.assembly;
    }

    public readonly ItemList Items;

    public void AddItem(ItemExport item) {
        item.Mod = this;
        Items.Add(item);
    }

    public void Export(string basePath) {
        var dir = $"{basePath}/{Id}";
        DirAccess.MakeDirRecursiveAbsolute(dir);
        var file = FileAccess.Open($"{dir}/items.json", FileAccess.ModeFlags.Write);
        file.StoreString(JsonSerializer.Serialize(Items, new JsonSerializerOptions { WriteIndented = true, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping }));
        file.Close();
    }
}