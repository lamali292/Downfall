using System.Text;
using System.Text.Json.Serialization;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;

namespace DataGen.DataGenCode.Exporter;

public class CardExport : ItemExport, IImageExport {
    public static readonly PackedScene CardScene = GD.Load<PackedScene>(NCard._scenePath);
    private static readonly Vector2I ImgSize = new(734, 916);

    private readonly CardModel _model;

    public CardExport(CardModel model, int upgrades) {
        Assembly = model.GetType().Assembly;
        _model = model.ToMutable();
        Upgrades = upgrades;
        for (var i = 0; i < Upgrades; i++)
            _model.UpgradeInternal();
    }

    [JsonInclude][JsonPropertyName("id")]
    public virtual string Id => _model.Id.Entry;
    [JsonInclude][JsonPropertyName("name")]
    public string Name => StripBbCodeTags(_model.Title, _model);
    [JsonInclude][JsonPropertyName("color")]
    public string Color => _model.VisualCardPool is ICustomEnergyIconPool && _model.VisualCardPool.EnergyColorName != "colorless" ? _model.VisualCardPool.EnergyColorName.ToLower() : _model.VisualCardPool.Title.ToLower();
    [JsonInclude][JsonPropertyName("rarity")]
    public string Rarity => _model.Rarity.ToString();
    [JsonInclude][JsonPropertyName("type")]
    public string Type => _model.Type.ToString();
    [JsonInclude][JsonPropertyName("cost")]
    public string Cost {
        get {
            if (_model.EnergyCost.CostsX) return "X";
            var cost = _model.EnergyCost.GetWithModifiers(CostModifiers.None);
            return cost == -1 ? "" : cost.ToString();
        }
    }
    [JsonInclude][JsonPropertyName("description")]
    public string Description => StripBbCodeTags(_model.GetDescriptionForPile(PileType.None), _model);
    [JsonInclude][JsonPropertyName("upgrades")]
    public readonly int Upgrades;
    [JsonIgnore] 
    private CardExport? UpgradedVersion => Upgrades >= _model.MaxUpgradeLevel ? null : new(_model.CanonicalInstance, Upgrades + 1);
    private string UpgradeDesc => UpgradedVersion is { } up ? up.Description : Description;
    [JsonIgnore]
    public string TextAndUpgrade => ProcessCombinedDescription(CombineDescriptions(Description, UpgradeDesc, TextMode.Normal), TextMode.Normal);
    [JsonIgnore]
    public string TextWikiData => ProcessCombinedDescription(CombineDescriptions(Description, UpgradeDesc, TextMode.WikiData), TextMode.WikiData);
    [JsonIgnore]
    public string TextWikiFormat => ProcessCombinedDescription(CombineDescriptions(Description, UpgradeDesc, TextMode.WikiFormat), TextMode.WikiFormat);


    [JsonInclude][JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)][JsonPropertyName("starCost")]
    public string? StarCost => _model.HasStarCostX ? "X" : _model.CanonicalStarCost == -1 ? null : _model.CanonicalStarCost.ToString();
    [JsonInclude][JsonIgnore(Condition=JsonIgnoreCondition.WhenWritingNull)][JsonPropertyName("tinkerTimeRider")]
    public virtual int? TinkerTimeRider => null;

    private enum TextMode { Normal, WikiData, WikiFormat }

    private static string CombineDescriptions(string a, string b, TextMode mode) {
        static string PreProcessText(string a, TextMode mode) {
            a = a.Replace("."," .").Replace(","," ,").Replace("\n", " \n ");
            if (mode == TextMode.WikiData || mode == TextMode.WikiFormat)
                a = a.Replace("[E]", "<E>");
            return a;
        }

        static int WordCost(string aW, string bW, TextMode mode) {
            if (aW == bW) return 0;
            if (mode != TextMode.WikiData && bW == aW + "s") return 10;
            return 21;
        }

        static string WordReplacement(string aW, string bW, TextMode mode) {
            if (aW == bW) return aW;
            if (mode != TextMode.WikiData && bW == aW + "s") return aW + "(s)";
            return aW + " (" + bW + ")";
        }

        // Combine description with upgrade description
        if (a == b && mode == TextMode.Normal) return a;
        // prepare punctuation, so we count it as separate words
        a = PreProcessText(a, mode);
        b = PreProcessText(b, mode);
        // Split input into words
        var aWords = a.Split(' ');
        var bWords = b.Split(' ');
        // Use the standard sequence alignment algorithm (Needleman–Wunsch)
        const int insertA = 10;
        const int insertB = 10;
        var score = new int[aWords.Length+1, bWords.Length+1];
        for (var aI=0; aI <= aWords.Length; aI++) {
            score[aI, 0] = aI * insertA;
        }
        for (var bI=0; bI <= bWords.Length; bI++) {
            score[0, bI] = bI * insertB;
        }
        for (var aI=1; aI <= aWords.Length; aI++) {
            for (var bI=1 ; bI <= bWords.Length ; bI++) {
                score[aI, bI] = Math.Min(score[aI-1, bI] + insertA,
                                Math.Min(score[aI, bI-1] + insertB,
                                        score[aI-1, bI-1] + WordCost(aWords[aI-1],bWords[bI-1],mode)));
            }
        }
        // Now return the optimal alignment, first in reverse order
        List<string> words = [];
        List<char> source = [];
        int ai=aWords.Length, bi=bWords.Length;
        while (ai > 0 && bi > 0) {
            var acost       = score[ai-1,bi] + insertA;
            var bcost       = score[ai,bi-1] + insertB;
            var replacecost = score[ai-1,bi-1] + WordCost(aWords[ai-1],bWords[bi-1],mode);
            if (bcost <= acost && bcost <= replacecost) {
                words.Add(bWords[bi-1]);
                source.Add('b');
                bi--;
            } else if (acost <= replacecost) {
                words.Add(aWords[ai-1]);
                source.Add('a');
                ai--;
            } else {
                words.Add(WordReplacement(aWords[ai-1],bWords[bi-1],mode));
                source.Add('c');
                ai--; bi--;
            }
        }
        while (bi > 0) {
            words.Add(bWords[bi-1]);
            source.Add('b');
            bi--;
        }
        while (ai > 0) {
            words.Add(aWords[ai-1]);
            source.Add('a');
            ai--;
        }
        // Now reverse
        words.Reverse();
        source.Reverse();
        // Add parentheses to destinguish the sources
        // We keep track of which source we are taking words from ('a', 'b', or a combination 'c')
        char prev = 'c';
        int astart = 0;
        StringBuilder builder = new();
        if (mode == TextMode.Normal || mode == TextMode.WikiFormat) {
            for (int i = 0; i < words.Count; i++) {
                if (i > 0) builder.Append(' ');
                if (source[i] == 'a' && prev != 'a') astart = i;
                if (source[i] != 'b' && prev == 'b') builder.Append(") ");
                if (source[i] == 'b' && prev != 'b') builder.Append('(');
                if (source[i] == 'c' && prev == 'a') {
                    // a deletion not followed by an insertion. For example "Exhaust. (not Exhaust.)".
                    builder.Append("(not");
                    for (int j = astart ; j < i ; j++) {
                        builder.Append(' ');
                        builder.Append(words[j]);
                    }
                    builder.Append(")");
                }
                prev = source[i];
                // is this a keyword?
                builder.Append(words[i]);
            }
            if (prev == 'b') builder.Append(')');
            if (prev == 'a') {
                builder.Append(" (not");
                for (int j = astart ; j < words.Count ; j++) {
                    builder.Append(' ');
                    builder.Append(words[j]);
                }
                builder.Append(')');
            }
        } else {
            for (var i = 0 ; i < words.Count ; i++) {
                switch (source[i])
                {
                    case 'c' when prev == 'a':
                        builder.Append("|] ");
                        break;
                    case 'c' when prev == 'b':
                        builder.Append("] ");
                        break;
                    case 'b' when prev == 'a':
                        builder.Append("|");
                        break;
                    case 'b' when prev == 'c':
                    {
                        builder.Append(i > 0 ? "| " : "|");
                        break;
                    }
                    case 'a' when prev == 'c':
                    {
                        builder.Append(i > 0 ? " [" : "[");
                        break;
                    }
                    case 'a' when prev == 'b':
                        builder.Append("] [");
                        break;
                    default:
                    {
                        if (i > 0) builder.Append(" ");
                        break;
                    }
                }
                prev = source[i];
                // is this a keyword?
                builder.Append(words[i]);
            }
            switch (prev)
            {
                case 'b':
                    builder.Append(']');
                    break;
                case 'a':
                    builder.Append('|');
                    break;
            }
        }
        // Join and remove unnecesary spaces
        var replace = builder.ToString().Replace(" .", ".").Replace(" ,", ",").Replace(" \n ","\n");
        return mode == TextMode.WikiData ? replace.Replace(" ]","]").Replace("[ ","[") : replace.Replace(" )",")").Replace("( ","(");
    }

    private string ProcessCombinedDescription(String description, TextMode textMode) {
        description = description.Replace(" \n] ", "\n]").Replace(" [\n ", "[\n").Replace("\n", "\\n");
        do {
            var start = description.IndexOf('[');
            if (description.Length <= start + 8 || description[start + 1] != '#' ||
                description[start + 8] != ']') continue;
            var code = description.Substring(start + 1, start + 8);

            description = (start > 0 ? description[..start] : "") + (textMode == TextMode.Normal ? "<span style=\"color:" + code + "\">" : "") + description[(start + 9)..];

            if (textMode == TextMode.Normal) {
                var braces = description.IndexOf("[]", StringComparison.Ordinal);
                var nextSpace = description.IndexOf(' ', start + 28);
                if (nextSpace >= 0 && nextSpace < braces) {
                    description = description[..nextSpace] + "</span>" + description[nextSpace..];
                } else if (braces >= 0) {
                    description = description.ReplaceFirst("\\[]", "</span>");
                } else {
                    description += "</span>";
                }
            } else {
                description = description.ReplaceFirst("\\[]", "");
            }
        } while (false);

        return description;
    }

    public ViewportManager.DrawRequest[] ExportImg(ExportConfig config) {
        return [Request()];

        ViewportManager.DrawRequest Request() => new(ImgSize, $"card-images/{(Upgrades == 0 ? Id : $"{Id}Plus{Upgrades}")}", null, drawer => {
            var card = CardScene.Instantiate<NCard>();
            drawer.AddChild(card);
            card.Scale = Vector2.One * 2f;
            card.Modulate = Colors.White;
            card.Position = (Vector2)ImgSize / 2f;
            card.Model = _model;
            card.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
        });
    }

    public static List<CardExport> FindAll() => [
        ..ModelDb.AllCards
            .Where(m => m.ShouldShowInCardLibrary)
            .SelectMany(m => Enumerable.Range(0, Math.Min(m.MaxUpgradeLevel, 1) + 1)
                .Select(u => new CardExport(m, u))),
    ];
}