using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace ImageGen;

public class SyncCardSheets
{
    private static readonly Dictionary<string, int> RarityOrder = new()
    {
        { "Basic", 0 },
        { "Common", 1 },
        { "Uncommon", 2 },
        { "Rare", 3 },
        { "Ancient", 4 }
    };

    private readonly string _serviceAccount;

    public SyncCardSheets(string scriptDir)
    {
        _serviceAccount = Path.Join(scriptDir, "service_account.json");
    }

    public void Run(string cardsJsonPath, string sheetId)
    {
        if (!File.Exists(_serviceAccount))
        {
            Console.WriteLine($"  [skip] service_account.json not found at {_serviceAccount}");
            return;
        }

        if (!File.Exists(cardsJsonPath))
        {
            Console.WriteLine($"  [skip] cards.json not found at {cardsJsonPath}");
            return;
        }

        Console.WriteLine("  Reading cards.json...");
        var allCards = JsonSerializer.Deserialize<List<CardEntry>>(File.ReadAllText(cardsJsonPath),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

        var baseCards = allCards.Where(c => c.Upgrades == 0).ToList();
        var upgraded = allCards
            .Where(c => c.Upgrades > 0)
            .DistinctBy(c => c.Id)
            .ToDictionary(c => c.Id);

        var credential = CredentialFactory.FromFile<ServiceAccountCredential>(_serviceAccount)
            .ToGoogleCredential()
            .CreateScoped(SheetsService.Scope.Spreadsheets);
        var service = new SheetsService(new BaseClientService.Initializer
            { HttpClientInitializer = credential, ApplicationName = "ImageGen" });

        var spreadsheet = service.Spreadsheets.Get(sheetId).Execute();

        foreach (var group in baseCards.GroupBy(c => c.Mod ?? "Unknown"))
        {
            var tabName = group.Key;
            Console.WriteLine($"  Syncing '{tabName}'...");

            var sorted = group
                .OrderBy(c => RarityOrder.TryGetValue(c.Rarity, out var o) ? o : 99)
                .ThenBy(c => c.Name)
                .ToList();

            var rows = new List<IList<object>>();
            var num = 1;
            foreach (var card in sorted)
            {
                upgraded.TryGetValue(card.Id, out var up);
                rows.Add([
                    num++,
                    card.Name,
                    card.Rarity,
                    card.Type,
                    MergeDiff(card.Cost, up?.Cost),
                    MergeDescription(card.Description, up?.Description)
                ]);
            }

            var tabSheetId = GetOrCreateSheet(service, spreadsheet, sheetId, tabName);

            service.Spreadsheets.Values.Clear(new ClearValuesRequest(), sheetId, tabName).Execute();

            string[] header = ["#", "Name", "Rarity", "Type", "Cost", "Description"];
            var data = new List<IList<object>> { header.Cast<object>().ToList() };
            data.AddRange(rows);

            var writeReq = service.Spreadsheets.Values.Update(
                new ValueRange { Values = data },
                sheetId, $"'{tabName}'!A1");
            writeReq.ValueInputOption =
                SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
            writeReq.Execute();
            // return;
            service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest
            {
                Requests =
                [
                    new Request
                    {
                        RepeatCell = new RepeatCellRequest
                        {
                            Range = new GridRange { SheetId = tabSheetId, StartRowIndex = 0, EndRowIndex = 1 },
                            Cell = new CellData
                                { UserEnteredFormat = new CellFormat { TextFormat = new TextFormat { Bold = true } } },
                            Fields = "userEnteredFormat.textFormat.bold"
                        }
                    },
                    new Request
                    {
                        UpdateSheetProperties = new UpdateSheetPropertiesRequest
                        {
                            Properties = new SheetProperties
                            {
                                SheetId = tabSheetId,
                                GridProperties = new GridProperties { FrozenRowCount = 1 }
                            },
                            Fields = "gridProperties.frozenRowCount"
                        }
                    },
                    new Request
                    {
                        RepeatCell = new RepeatCellRequest
                        {
                            Range = new GridRange { SheetId = tabSheetId, StartColumnIndex = 5, EndColumnIndex = 6 },
                            Cell = new CellData { UserEnteredFormat = new CellFormat { WrapStrategy = "CLIP" } },
                            Fields = "userEnteredFormat.wrapStrategy"
                        }
                    },
                    new Request
                    {
                        AutoResizeDimensions = new AutoResizeDimensionsRequest
                        {
                            Dimensions = new DimensionRange
                            {
                                SheetId = tabSheetId,
                                Dimension = "COLUMNS",
                                StartIndex = 0,
                                EndIndex = 6
                            }
                        }
                    }
                ]
            }, sheetId).Execute();

            var rarityRequests = new List<Request>();
            for (var i = 0; i < sorted.Count; i++)
            {
                var card = sorted[i];
                var color = card.Rarity switch
                {
                    "Basic" => new Color { Red = 0.85f, Green = 0.85f, Blue = 0.85f },
                    "Common" => new Color { Red = 0.75f, Green = 0.88f, Blue = 1.0f },
                    "Uncommon" => new Color { Red = 0.75f, Green = 1.0f, Blue = 0.75f },
                    "Rare" => new Color { Red = 1.0f, Green = 0.95f, Blue = 0.6f },
                    "Ancient" => new Color { Red = 1.0f, Green = 0.85f, Blue = 0.4f },
                    _ => null
                };
                if (color == null) continue;

                rarityRequests.Add(new Request
                {
                    RepeatCell = new RepeatCellRequest
                    {
                        Range = new GridRange
                        {
                            SheetId = tabSheetId,
                            StartRowIndex = i + 1,
                            EndRowIndex = i + 2,
                            StartColumnIndex = 0,
                            EndColumnIndex = rows[i].Count
                        },
                        Cell = new CellData { UserEnteredFormat = new CellFormat { BackgroundColor = color } },
                        Fields = "userEnteredFormat.backgroundColor"
                    }
                });
            }

            foreach (var chunk in rarityRequests.Chunk(500))
                service.Spreadsheets.BatchUpdate(
                    new BatchUpdateSpreadsheetRequest { Requests = [..chunk] }, sheetId).Execute();

            Console.WriteLine($"  [{tabName}] {rows.Count} cards written.");
        }
    }

    private static int GetOrCreateSheet(SheetsService service, Spreadsheet spreadsheet, string sheetId, string name)
    {
        var existing = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == name);
        if (existing is not null)
            return (int)existing.Properties.SheetId!;

        var resp = service.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest
        {
            Requests =
            [
                new Request
                {
                    AddSheet = new AddSheetRequest { Properties = new SheetProperties { Title = name } }
                }
            ]
        }, sheetId).Execute();

        return (int)resp.Replies[0].AddSheet.Properties.SheetId!;
    }

    private static string MergeDiff(string base_, string? up)
    {
        return up == null || base_ == up ? base_ : $"{base_} ({up})";
    }

    private static string MergeDescription(string base_, string? up)
    {
        if (up == null) return Flatten(base_);
        var bFlat = Flatten(base_);
        var uFlat = Flatten(up);
        if (bFlat == uFlat) return bFlat;

        var bSentences = SplitSentences(bFlat);
        var uSentences = SplitSentences(uFlat);

        var diff = InlineDiffBuilder.Diff(
            string.Join("\n", bSentences),
            string.Join("\n", uSentences));

        var parts = new List<string>();
        var pendingDel = new List<string>();
        var pendingIns = new List<string>();

        void Flush()
        {
            // pair up deletions and insertions
            var count = Math.Max(pendingDel.Count, pendingIns.Count);
            for (var i = 0; i < count; i++)
            {
                var d = i < pendingDel.Count ? pendingDel[i] : null;
                var ins = i < pendingIns.Count ? pendingIns[i] : null;
                if (d == null)
                {
                    parts.Add($"({ins})");
                }
                else if (ins == null)
                {
                    parts.Add($"){d}(");
                }
                else if (Similarity(d, ins) > 0.2)
                {
                    parts.Add(MergeWords(d, ins));
                }
                else
                {
                    parts.Add($"){d}(");
                    parts.Add($"({ins})");
                }
            }

            pendingDel.Clear();
            pendingIns.Clear();
        }

        foreach (var line in diff.Lines)
            switch (line.Type)
            {
                case ChangeType.Unchanged:
                    Flush();
                    parts.Add(line.Text);
                    break;
                case ChangeType.Deleted:
                    pendingDel.Add(line.Text);
                    break;
                case ChangeType.Inserted:
                    pendingIns.Add(line.Text);
                    break;
            }

        Flush();

        return string.Join(" ", parts);
    }

    private static double Similarity(string a, string b)
    {
        var aWords = a.Split(' ').ToHashSet();
        var bWords = b.Split(' ').ToHashSet();
        var common = aWords.Count(w => bWords.Contains(w));
        return (double)common / Math.Max(aWords.Count, bWords.Count);
    }

    private static string MergeWords(string base_, string up)
    {
        if (base_ == up) return base_;

        var bw = base_.Split(' ');
        var uw = up.Split(' ');

        // Same length — zip directly
        if (bw.Length == uw.Length)
            return string.Join(" ", bw.Zip(uw, MergeWord))
                .Replace(" .", ".").Replace(" ,", ",")
                .Replace(" )", ")").Replace("( ", "(");

        // Different length — use DiffPlex at word level
        var diff = InlineDiffBuilder.Diff(
            string.Join("\n", bw),
            string.Join("\n", uw));

        var result = new List<string>();
        var pending = new List<string>();
        foreach (var line in diff.Lines)
            switch (line.Type)
            {
                case ChangeType.Unchanged:
                    FlushDeleted(result, pending);
                    result.Add(line.Text);
                    break;
                case ChangeType.Deleted:
                    pending.Add(line.Text);
                    break;
                case ChangeType.Inserted:
                    if (pending.Count > 0)
                    {
                        result.Add(MergeWord(pending[0], line.Text));
                        pending.RemoveAt(0);
                        FlushDeleted(result, pending);
                    }
                    else
                    {
                        result.Add($"({line.Text})");
                    }

                    break;
            }

        FlushDeleted(result, pending);

        return string.Join(" ", result)
            .Replace(" .", ".").Replace(" ,", ",")
            .Replace(" )", ")").Replace("( ", "(");
    }

    private static void FlushDeleted(List<string> parts, List<string> deleted)
    {
        foreach (var d in deleted) parts.Add($"){d}(");
        deleted.Clear();
    }

    private static string MergeWord(string a, string b)
    {
        if (a == b) return a;
        if (b == a + "+") return $"{a}(+)";
        var ac = a.TrimEnd('.', ',', '!', '?', ';');
        var bc = b.TrimEnd('.', ',', '!', '?', ';');
        var punctA = a.Length > ac.Length ? a[ac.Length..] : "";
        if (ac == bc) return b;
        if (bc == ac + "+") return $"{ac}(+){punctA}";
        // plural: card -> cards
        if (bc == ac + "s") return $"{ac}(s){punctA}";
        if (int.TryParse(ac, out _) && int.TryParse(bc, out _)) return $"{ac} ({bc}){punctA}";
        if (ac.Replace("[E]", "").Length == 0 && bc.Replace("[E]", "").Length == 0)
            return $"{ac}({bc[ac.Length..]}){punctA}";
        return $"{ac} ({bc}){punctA}";
    }

    private static string[] SplitSentences(string s)
    {
        return Regex.Split(s, @"(?<=\.)\s+")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
    }

    private static string Flatten(string s)
    {
        return s.Replace("\n", " ").Trim();
    }

    // ReSharper disable UnusedAutoPropertyAccessor.Local
    // ReSharper disable AutoPropertyCanBeMadeGetOnly.Local
    private class CardEntry
    {
        [JsonPropertyName("id")] public string Id { get; set; } = "";
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("rarity")] public string Rarity { get; set; } = "";
        [JsonPropertyName("type")] public string Type { get; set; } = "";
        [JsonPropertyName("cost")] public string Cost { get; set; } = "";
        [JsonPropertyName("description")] public string Description { get; set; } = "";
        [JsonPropertyName("upgrades")] public int Upgrades { get; set; }
        [JsonPropertyName("mod")] public string? Mod { get; set; }
    }
    // ReSharper restore AutoPropertyCanBeMadeGetOnly.Local
    // ReSharper restore UnusedAutoPropertyAccessor.Local
}