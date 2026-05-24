using System.Text.Json.Serialization;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Potions;

namespace DataGen.DataGenCode.Exporter;

public class PotionExport : ItemExport, IImageExport
{
    private static readonly Vector2I ImgSize = new(64, 64);

    private readonly PotionModel _model;

    public PotionExport(PotionModel model)
    {
        Assembly = model.GetType().Assembly;
        _model = model.ToMutable();
    }

    [JsonInclude] [JsonPropertyName("id")] public string Id => _model.Id.Entry;

    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name => _model.Title.GetFormattedText();

    [JsonInclude]
    [JsonPropertyName("color")]
    public string Pool => _model.Pool.EnergyColorName.ToLower();

    [JsonInclude]
    [JsonPropertyName("rarity")]
    public string Rarity => _model.Rarity.ToString();

    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description => StripBbCodeTags(_model.DynamicDescription.GetFormattedText(), _model);

    public ViewportManager.DrawRequest[] ExportImg(ExportConfig config)
    {
        return
        [
            new ViewportManager.DrawRequest(ImgSize, $"potions/{Id}", null, drawer =>
            {
                var potion = NPotion.Create(_model);
                if (potion == null) return;
                drawer.AddChild(potion);
                potion.Modulate = Colors.White;
                potion.Position = (Vector2)ImgSize / 2f - potion.Size / 2f;
                potion.Model = _model;
            })
        ];
    }

    public static List<PotionExport> FindAll()
    {
        return [..ModelDb.AllPotions.Select(m => new PotionExport(m))];
    }
}