using System.Text.Json.Serialization;
using Godot;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.InspectScreens;

namespace DataGen.DataGenCode.Exporter;

public class RelicExport : ItemExport, IImageExport
{
    private const float TipScale = 1f;
    private const float TipCardGap = 10f;
    private const float TipTopMargin = 20f;
    private static readonly Vector2I ImgSize = new(200, 200);

    private static TextureRect? _exampleTexRect;

    private readonly RelicModel _model;

    public RelicExport(RelicModel model)
    {
        Assembly = model.GetType().Assembly;
        _model = model;
    }

    [JsonInclude] [JsonPropertyName("id")] public string Id => _model.Id.Entry;

    [JsonInclude]
    [JsonPropertyName("name")]
    public string Name => _model.Title.GetFormattedText();

    [JsonInclude]
    [JsonPropertyName("pool")]
    public string Pool => _model.Pool.EnergyColorName.ToLower();

    [JsonInclude]
    [JsonPropertyName("ancient")]
    public string? Ancient => ModelDb.AllAncients
        .FirstOrDefault(m => m.AllPossibleOptions.Select(static o => o.Relic?.Id.Entry).Contains(_model.Id.Entry))?.Title
        .GetRawText();

    [JsonInclude]
    [JsonPropertyName("tier")]
    public string Rarity => _model.Rarity.ToString();

    [JsonInclude]
    [JsonPropertyName("description")]
    public string Description => StripBbCodeTags(_model.DynamicDescription.GetFormattedText(), _model);

    [JsonInclude]
    [JsonPropertyName("flavorText")]
    public string Flavor => StripBbCodeTags(_model.Flavor.GetFormattedText(), _model);

    public ViewportManager.DrawRequest[] ExportImg(ExportConfig config)
    {
        ViewportManager.DrawRequest relicImg = new(ImgSize, $"relics/{Id}", null, drawer =>
        {
         
            if (_exampleTexRect == null)
            {
                var screen = NInspectRelicScreen.Create();
                if (screen == null) return;
                _exampleTexRect = screen.GetNode<TextureRect>("%RelicImage");
            }

            var textureRect = (TextureRect)_exampleTexRect.Duplicate();
            drawer.AddChild(textureRect);
            textureRect.SelfModulate = Colors.White;
            textureRect.Texture = _model.BigIcon;
            textureRect.Position = (Vector2)ImgSize / 2f - textureRect.Size / 2f;
        });
        Control hoverTipParent = new();
        var tipSet = NHoverTipSet.CreateAndShow(hoverTipParent, HoverTipFactory.FromRelic(_model));
        tipSet?.Reparent(hoverTipParent);
        tipSet?._cardHoverTipContainer.LayoutResizeAndReposition(Vector2.Zero, HoverTipAlignment.Center);
        var sizeX = tipSet?._textHoverTipContainer.Size.X ?? 0;
        var sizeY = tipSet?._textHoverTipContainer.Size.Y ?? 0;
        var tipSize = (Vector2I)(new Vector2(
            sizeX + (sizeX > 0 ? TipCardGap : 0) +
            sizeX,
            Mathf.Max(sizeY,
                sizeY + TipTopMargin * TipScale)) * TipScale);
        ViewportManager.DrawRequest tipImg = new(tipSize, $"relic-tips/{Id}", null, drawer =>
        {
            drawer.AddChild(hoverTipParent);
            if (tipSet == null) return;
            tipSet.Scale *= TipScale;
            tipSet.PivotOffset = Vector2.Zero;
            tipSet._cardHoverTipContainer.Position =
                new Vector2(tipSet._textHoverTipContainer.Size.X + TipCardGap, TipTopMargin);
        });
        return [relicImg, tipImg];
    }

    public static List<RelicExport> FindAll()
    {
        return [..ModelDb.AllRelics.Select(m => new RelicExport(m))];
    }
}