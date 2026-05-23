using System.Text.Json.Serialization;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace DataGen.DataGenCode.Exporter;

public class EnchantmentExport : ItemExport, IImageExport
{
    private static readonly Vector2I ImgSize = new(68, 54);

    private readonly EnchantmentModel _model;

    public EnchantmentExport(EnchantmentModel model)
    {
        Assembly = model.GetType().Assembly;
        _model = model.ToMutable();
        _model.Amount = 999;
    }

    [JsonInclude] [JsonPropertyName("id")] private string Id => _model.Id.Entry;

    [JsonInclude]
    [JsonPropertyName("name")]
    private string Name => _model.Title.GetFormattedText();

    [JsonInclude]
    [JsonPropertyName("description")]
    private string Description =>
        StripBbCodeTags(_model.DynamicDescription.GetFormattedText().Replace("999", "N"), _model);

    public ViewportManager.DrawRequest[] ExportImg(ExportConfig config)
    {
        return
        [
            new ViewportManager.DrawRequest(ImgSize, $"enchantments/{Id}", null, drawer =>
            {
                var cardModel = ModelDb.Get<UltimateDefend>().ToMutable();
                var card = CardExport.CardScene.Instantiate<NCard>();
                drawer.AddChild(card);
                card.Model = cardModel;
                //cardModel.Enchantment = model;
                card.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
                Control tab = new() { Size = Vector2.Zero };
                //card._enchantmentLabel.Modulate = Colors.Transparent;
                card._enchantmentTab.Show();
                card._enchantmentIcon.Texture = _model.Icon;
                card._enchantmentLabel.Visible = _model.ShowAmount;
                if (_model.ShowAmount)
                    card._enchantmentLabel.SetTextAutoSize("N");
                card._enchantmentTab.Reparent(tab);
                drawer.AddChild(tab);
                tab.Position = new Vector2(164, 161);
                card.QueueFree();
            })
        ];
    }

    public static List<EnchantmentExport> FindAll()
    {
        return [..ModelDb.DebugEnchantments.Select(static m => new EnchantmentExport(m))];
        //[..new List<Assembly>([typeof(NGame).Assembly, ..ModManager.AllMods.Select(static m => m.assembly).Where(static a => a != null)]).SelectMany(static a => a.GetTypes()).Where(static t => t.IsAssignableTo(typeof(EnchantmentModel)) && !t.IsAbstract).Select(static t => new EnchantmentExport((EnchantmentModel)ModelDb.Get(t)))];
    }
}