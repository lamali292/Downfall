using Downfall.DownfallCode.Voting;
using Godot;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.DownfallCode.Voting;

public partial class NArtVotingContainer : Control
{
    private const string VoteCardScenePath = "res://Downfall/scenes/voting/art_row.tscn";

    private MegaRichTextLabel _title = null!;
    private HFlowContainer _content = null!;
    private PackedScene _voteCardScene = null!;
    private int _fillGeneration;

    private readonly Dictionary<string, List<ArtEntry>> _entryCache = new();

    [Signal] public delegate void EntryClickedEventHandler(string imagePath);

    public override void _Ready()
    {
        _title = GetNode<MegaRichTextLabel>("SubmissionsTitle");
        _content = GetNode<HFlowContainer>("SubmissionsScrollContainer/Mask/Content");
        _voteCardScene = PreloadManager.Cache.GetScene(VoteCardScenePath);
        ClearCards();
    }

    public async void Fill(ArtData artData)
    {
        var gen = ++_fillGeneration;
        ClearCards();

        var cardModel = artData.Card;
        if (cardModel == null)
        {
            GD.PrintErr($"No card model for {artData.ModelId} — skipping");
            _title.Text = "?";
            return;
        }

        _title.Text = cardModel.Title;

        var entries = await GetEntriesFor(artData);
        if (gen != _fillGeneration || entries == null)
            return;

        foreach (var entry in entries)
        {
            var card = _voteCardScene.Instantiate<NVoteCard>();
            _content.AddChild(card);
            card.SetEntry(entry);
            card.ScoreChanged += SortByScore;
            card.CardClicked += path => EmitSignal(SignalName.EntryClicked, path);
        }

        SortByScore();
    }

    private void SortByScore()
    {
        var cards = _content.GetChildren().OfType<NVoteCard>()
            .OrderByDescending(c => c.Score)
            .ToList();

        for (var i = 0; i < cards.Count; i++)
            _content.MoveChild(cards[i], i);
    }

    private async Task<List<ArtEntry>?> GetEntriesFor(ArtData artData)
    {
    
        if (artData.Entries is { Count: > 0 })
            return artData.Entries;

        var key = artData.Id ?? artData.ModelId.ToString();
        
        if (_entryCache.TryGetValue(key, out var cached))
            return cached;
        
        var fetched = await FetchFromDatabase(key);
        if (fetched == null) return fetched;
        _entryCache[key] = fetched;
        artData.Entries = fetched;   
        return fetched;
    }

    private async Task<List<ArtEntry>?> FetchFromDatabase(string categoryId)
    {
        return await VotingApi.Instance.GetSubmissions(categoryId);
    }

    private void ClearCards()
    {
        foreach (var child in _content.GetChildren())
        {
            _content.RemoveChild(child);
            child.QueueFree();
        }
    }
}