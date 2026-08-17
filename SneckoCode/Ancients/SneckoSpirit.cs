using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ancients;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Relics;

namespace Snecko.SneckoCode.Ancients;

public class SneckoSpirit() : CustomAncientModel(logDialogueLoad: true)
{
    private List<(CharacterModel left, CharacterModel right)> _pairs = [];
    private List<CharacterModel> _chosen = [];
    public Dictionary<EventOption, CharacterModel> OptionCharacters = new();
    
    public override bool IsValidForAct(ActModel act) => false;

    protected override OptionPools MakeOptionPools => new(MakePool(Array.Empty<RelicModel>()));
    public override IEnumerable<EventOption> AllPossibleOptions => Array.Empty<EventOption>();

    public override string CustomScenePath => "res://Snecko/scenes/ancient/snecko_spirit.tscn";
    public override string CustomMapIconPath => "res://Snecko/images/ancients/snecko_spirit_node.png";
    public override string CustomMapIconOutlinePath => "res://Snecko/images/ancients/snecko_spirit_node_outline.png";
    public override string CustomRunHistoryIconPath => "res://Snecko/images/ancients/snecko_spirit_history.png";
    public override string CustomRunHistoryIconOutlinePath => "res://Snecko/images/ancients/snecko_spirit_history_outline.png";

    public override Godot.Color ButtonColor => new(0.06f, 0.0f, 0.08f, 0.5f);
    public override Godot.Color DialogueColor => new("512E66");
    

    public IReadOnlyList<AncientDialogueLine> CurrentTranscriptLines => BuildTranscript();

    private bool IsSnecko => Owner!.Character is Core.Snecko;

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        if (!IsSnecko)
            return []; // empty -> StartPreFinished() finishes it with the DONE page

        var others = ModelDb.AllCharacters.Where(c => c != Owner!.Character).ToList();
        if (others.Count < 6)
            return [];

        var six = others.TakeRandom(6, Rng).ToList();
        _pairs.Clear();
        _chosen.Clear();
        for (var i = 0; i < 3; i++)
            _pairs.Add((six[i * 2], six[i * 2 + 1]));

        return BuildPage(0);
    }

    private IReadOnlyList<EventOption> BuildPage(int page)
    {
        var (left, right) = _pairs[page];
        return new List<EventOption> { CharOption(left, page), CharOption(right, page) };
    }

    private EventOption CharOption(CharacterModel c, int page)
    {
        var opt = new EventOption(
            this,
            () => OnPicked(c, page),
            c.Title,
            c.CardsModifierDescription,
            $"pool_{page}_{c.Id.Entry}",
            []);
        OptionCharacters[opt] = c;
        return opt;
    }

    private async Task OnPicked(CharacterModel c, int page)
    {
        _chosen.Add(c);

        // Grant the relic for THIS choice, right now.
        var relic = (SneckoChoice) ModelDb.Relic<SneckoChoice>().ToMutable();
        relic.InitCharacter(c);
        await RelicCmd.Obtain(relic, Owner!);

        if (page + 1 < _pairs.Count)
        {
            SetEventState(
                L10NLookup($"{Id.Entry}.pages.PAGE_{page + 1}.description"),
                BuildPage(page + 1));
        }
        else
        {
            Done();
        }
    }
    protected override AncientDialogueSet DefineDialogues()
    {
        var set = base.DefineDialogues();
        return new AncientDialogueSet
        {
            FirstVisitEverDialogue = new AncientDialogue(new[] { "event:/sfx/npcs/snecko_spirit/hiss" }),
            CharacterDialogues = set.CharacterDialogues,
            AgnosticDialogues = set.AgnosticDialogues,
        };
    }

    private IReadOnlyList<AncientDialogueLine> BuildTranscript()
    {
        var lines = new List<AncientDialogueLine>();
        if (!IsSnecko) return lines;
        for (var page = 0; page <= _chosen.Count && page < 3; page++)
        {
            AppendDialogue(lines, "SPIRIT", page, null);
            if (page < _chosen.Count)
                AppendDialogue(lines, "ECHO", page, _chosen[page]);
        }
        return lines;
    }

    protected override void AfterCloned()
    {
        base.AfterCloned();
        _pairs = new();
        _chosen = new();
        OptionCharacters = new();
    }
    
    private void AppendDialogue(List<AncientDialogueLine> into, string charEntry, int index, CharacterModel? picked)
    {
        try
        {
            var sfx = charEntry == "SPIRIT" ? SpiritSfxForPage(index) : "";
            var dialogue = new AncientDialogue(sfx);
            dialogue.PopulateLines(Id.Entry, charEntry, index);
            foreach (var line in dialogue.Lines)
                if (picked != null && line.LineText != null)
                    line.LineText.Add("char", picked.Title);
            into.AddRange(dialogue.Lines);
        }
        catch (Exception e)
        {
            Log.Warn($"SneckoSpirit: failed to build dialogue line {Id.Entry}.talk.{charEntry}.{index}: {e.Message}");
        }
    }

    private string SpiritSfxForPage(int page) => "event:/sfx/npcs/snecko_spirit/hiss";
}