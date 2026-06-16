using MegaCrit.Sts2.Core.Entities.Cards;

namespace Downfall.DownfallCode.Voting;

using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Nodes.Cards;

public partial class NArtVotingCardContainer : Control
{
    private NCard? _card;
    private string _currentImagePath = "";

    public async void Fill(ArtData artData)
    {
        if (_card != null)
        {
            RemoveChild(_card);
            _card.QueueFree();
            _card = null;
        }
        _currentImagePath = "";

        var model = artData.Card;
        if (model == null) return;

        _card = NCard.Create(model);
        if (_card == null) return;

        AddChild(_card);
        _card.Position = NCard.defaultSize / 2f + new Vector2(100, 100);

        if (!_card.IsNodeReady())
            await ToSignal(_card, Node.SignalName.Ready);
        
        _card.UpdateVisuals(PileType.Deck, CardPreviewMode.Normal);
    }

    public async void UpdateImage(string imagePath)
    {
        if (_card == null || string.IsNullOrEmpty(imagePath)) return;
        if (imagePath == _currentImagePath) return;
        _currentImagePath = imagePath;

        var tex = await LoadTexture(imagePath);
        if (tex == null || _card == null || _currentImagePath != imagePath) return;
        _card.GetNode<TextureRect>("%Portrait").Texture = tex;
    }

    private async Task<Texture2D?> LoadTexture(string path)
    {
        if (NVoteCard.TextureCache.TryGetValue(path, out var cached))
            return cached;

        Texture2D? tex;
        if (path.StartsWith("res://"))
            tex = ResourceLoader.Exists(path) ? GD.Load<Texture2D>(path) : null;
        else if (path.StartsWith("http://") || path.StartsWith("https://"))
            tex = await Download(path);
        else if (Godot.FileAccess.FileExists(path))
        {
            var img = new Image();
            tex = img.Load(path) == Error.Ok ? ImageTexture.CreateFromImage(img) : null;
        }
        else
            tex = null;

        if (tex != null)
            NVoteCard.TextureCache[path] = tex;
        return tex;
    }

    private async Task<Texture2D?> Download(string url)
    {
        var http = new HttpRequest();
        AddChild(http);
        if (http.Request(url) != Error.Ok) { http.QueueFree(); return null; }

        var result = await ToSignal(http, HttpRequest.SignalName.RequestCompleted);
        http.QueueFree();

        var body = result[3].AsByteArray();
        var img = new Image();
        if (img.LoadPngFromBuffer(body) != Error.Ok &&
            img.LoadJpgFromBuffer(body) != Error.Ok &&
            img.LoadWebpFromBuffer(body) != Error.Ok)
            return null;

        return ImageTexture.CreateFromImage(img);
    }
}