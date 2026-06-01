using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace ImageGen;

public class SyncSheets
{
    private const string GithubRaw = "https://raw.githubusercontent.com/lamali292/Downfall/develop-2/ImageGen/images";

    private const int RowHeightPx = 130;
    private const int ImgColPx = 170;

    private static readonly Color ColMissing = Rgb(0.99f, 0.80f, 0.80f);
    private static readonly Color ColPlaceholder = Rgb(1.00f, 0.95f, 0.70f);
    private static readonly Color ColFinal = Rgb(0.80f, 0.95f, 0.80f);
    private static readonly Color ColTabMissing = Rgb(0.85f, 0.27f, 0.27f);
    private static readonly Color ColWhite = Rgb(1.0f, 1.0f, 1.0f);

    private static readonly string[] Header =
        ["Status", "Rarity", "Character", "File Name", "Title", "Description", "Image"];

    private readonly string _cacheFile;
    private readonly string _imagesDir;
    private readonly string _parent;

    private readonly string _scriptDir;
    private readonly string _serviceAccount;

    private readonly string _sheetId;

    public SyncSheets(string scriptDir, string sheetId)
    {
        _sheetId = sheetId;
        _scriptDir = scriptDir;
        _imagesDir = Path.Join(scriptDir, "images");
        _parent = Path.GetFullPath(Path.Join(scriptDir, ".."));
        _serviceAccount = Path.Join(scriptDir, "service_account.json");
        _cacheFile = Path.Join(scriptDir, ".sheets_cache.json");
        Console.WriteLine($"  Parent: {_parent}");
        Console.WriteLine(
            $"  Dirs: {string.Join(", ", Directory.EnumerateDirectories(_parent).Select(Path.GetFileName))}");
    }

    public void Run()
    {
        Console.WriteLine("[1/7] Discovering characters...");
        var characters = DiscoverCharacters();
        Console.WriteLine($"  Found {characters.Count}: {string.Join(", ", characters.Keys.Order())}");

        Console.WriteLine("\n[2/7] Indexing card images...");
        var cardImgs = IndexImages(["cards", "cards_beta"], false);

        Console.WriteLine("\n[3/7] Indexing power images...");
        var powerImgs = IndexImages(["powers", "powers_beta"], true);

        Console.WriteLine("\n[4/7] Collecting card code...");
        var cardsCode = CollectCode(characters, "Cards", false);

        Console.WriteLine("\n[5/7] Collecting power code...");
        var powersCode = CollectCode(characters, "Powers", true);

        Console.WriteLine("\n[6/7] Loading localization...");
        var cardLoc = LoadLoc(characters, "cards.json");
        var powerLoc = LoadLoc(characters, "powers.json");

        Console.WriteLine("\nBuilding rows...");
        var cardRows = BuildRows(cardsCode, cardImgs, cardLoc, false);
        var powerRows = BuildRows(powersCode, powerImgs, powerLoc, true);

        Console.WriteLine("\n[7/7] Connecting to Google Sheets...");
        if (!File.Exists(_serviceAccount))
        {
            Console.WriteLine($"  [skip] service_account.json not found at {_serviceAccount}");
            return;
        }

        var credential = CredentialFactory.FromFile<ServiceAccountCredential>(_serviceAccount)
            .ToGoogleCredential()
            .CreateScoped(SheetsService.Scope.Spreadsheets);
        var service = new SheetsService(new BaseClientService.Initializer
            { HttpClientInitializer = credential, ApplicationName = "ImageGen" });
        Console.WriteLine("  Connected.");

        var cache = LoadDict(_cacheFile);
        SyncSheet(service, cache, "⚠ Needs Work (Cards)", cardRows, ColTabMissing);
        SyncSheet(service, cache, "⚠ Needs Work (Powers)", powerRows, ColTabMissing);

        File.WriteAllText(_cacheFile,
            JsonSerializer.Serialize(cache, new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine("\nDone!");
    }

    private Dictionary<string, CharInfo> DiscoverCharacters()
    {
        var result = new Dictionary<string, CharInfo>();
        foreach (var entry in Directory.EnumerateDirectories(_parent))
        {
            var entryName = Path.GetFileName(entry);
            if (!entryName.EndsWith("Code")) continue;
            var charId = entryName[..^4].ToLower();
            var project = entryName[..^4]; // e.g. "Automaton"
            result[charId] = new CharInfo(entry, project, _parent);
            Console.WriteLine($"    {charId} -> {Path.GetRelativePath(_parent, entry)}");
        }

        return result;
    }

    private Dictionary<string, ImageEntry> IndexImages(string[] subdirs, bool isPower)
    {
        var index = new Dictionary<string, ImageEntry>();
        foreach (var subdirName in subdirs)
        {
            var subdirPath = Path.Join(_imagesDir, subdirName);
            if (!Directory.Exists(subdirPath))
            {
                Console.WriteLine($"    [skip] not found: {subdirPath}");
                continue;
            }

            var found = 0;
            foreach (var abs in Directory.EnumerateFiles(subdirPath, "*.png", SearchOption.AllDirectories))
            {
                var relFromImagesDir = Path.GetRelativePath(_imagesDir, abs).Replace('\\', '/');
                var stem = Path.GetFileNameWithoutExtension(abs);
                var norm = Normalize(stem, isPower);

                var status = subdirName.Contains("beta") ? "placeholder" : "final";

                if (index.TryGetValue(norm, out var existing))
                {
                    if (existing.Status == "final") continue;
                    if (existing.Status == "placeholder" && status == "missing") continue;
                }

                index[norm] = new ImageEntry(status, subdirName,
                    Path.GetRelativePath(subdirPath, abs).Replace('\\', '/'));
                found++;
            }

            Console.WriteLine($"    [{subdirName}] {found} PNGs.");
        }

        Console.WriteLine($"  Total unique keys: {index.Count}");
        return index;
    }

    private Dictionary<string, Dictionary<string, CodeEntry>> CollectCode(
        Dictionary<string, CharInfo> characters, string assetType, bool isPower)
    {
        var result = new Dictionary<string, Dictionary<string, CodeEntry>>();
        var total = 0;
        foreach (var (charId, info) in characters.OrderBy(kv => kv.Key))
        {
            var basePath = Path.Join(info.CodeDir, assetType);
            if (!Directory.Exists(basePath))
            {
                Console.WriteLine($"    [skip] no {assetType} folder for {charId}");
                continue;
            }

            var entries = new Dictionary<string, CodeEntry>();
            foreach (var file in Directory.EnumerateFiles(basePath, "*.cs", SearchOption.AllDirectories))
            {
                if (file.Contains($"{Path.DirectorySeparatorChar}Abstract{Path.DirectorySeparatorChar}")) continue;
                var rel = Path.GetRelativePath(basePath, Path.GetDirectoryName(file)!);
                var rarity = rel == "." ? "" : rel.Split(Path.DirectorySeparatorChar)[0].ToLower();
                var name = Path.GetFileNameWithoutExtension(file);
                var snake = ToSnake(name);
                if (isPower) snake = Regex.Replace(snake, "_power$", "");
                entries[Normalize(name, isPower)] = new CodeEntry(snake, rarity, info.Project.ToUpper());
                total++;
            }

            result[charId] = entries;
            Console.WriteLine($"    [{charId}] {entries.Count} .cs files. (prefix: {info.Project.ToUpper()}-)");
        }

        Console.WriteLine($"  Total: {total} entries across {result.Count} characters.");
        return result;
    }

    // ── Localization ──────────────────────────────────────────────────────────

    private Dictionary<string, string> LoadLoc(Dictionary<string, CharInfo> characters, string filename)
    {
        var merged = new Dictionary<string, string>();
        foreach (var (charId, info) in characters.OrderBy(kv => kv.Key))
        {
            string[] candidates =
            [
                Path.Join(info.ProjectPath, info.Project, "localization", "eng", filename),
                Path.Join(info.ProjectPath, "localization", "eng", filename)
            ];
            var found = false;
            foreach (var path in candidates)
            {
                if (!File.Exists(path)) continue;
                var data = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path)) ??
                           new Dictionary<string, string>();
                foreach (var kv in data) merged[kv.Key] = kv.Value;
                Console.WriteLine($"    [{charId}] {data.Count} keys from {Path.GetRelativePath(_parent, path)}");
                found = true;
                break;
            }

            if (!found) Console.WriteLine($"    [{charId}] No {filename} found.");
        }

        Console.WriteLine($"  Total merged keys: {merged.Count}");
        return merged;
    }

    // ── Build Rows ────────────────────────────────────────────────────────────

    private List<List<string>> BuildRows(
        Dictionary<string, Dictionary<string, CodeEntry>> byChar,
        Dictionary<string, ImageEntry> imgIndex,
        Dictionary<string, string> loc,
        bool isPower)
    {
        var rows = new List<List<string>>();
        var skippedFinal = 0;
        var missingLoc = new List<string>();

        foreach (var charId in byChar.Keys.Order())
            foreach (var norm in byChar[charId].Keys.Order())
            {
                var (snake, rarity, locPrefix) = byChar[charId][norm];
                if (!isPower && string.IsNullOrEmpty(rarity)) continue;

                imgIndex.TryGetValue(norm, out var entry);
                var status = entry?.Status.ToUpper() ?? "MISSING";
                if (status == "FINAL")
                {
                    skippedFinal++;
                    continue;
                }

                var url = entry is not null
                    ? $"=IMAGE(\"{GithubRaw}/{entry.Subdir}/{entry.RelFromSubdir}\")"
                    : "";

                var locKey = snake.ToUpper();
                if (isPower && !locKey.EndsWith("_POWER")) locKey += "_POWER";

                var title = loc.GetValueOrDefault($"{locPrefix}-{locKey}.title", "");
                var desc = loc.GetValueOrDefault($"{locPrefix}-{locKey}.description", "");
                if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(desc)) missingLoc.Add($"{locPrefix}-{locKey}");

                var rarityDisplay = string.IsNullOrEmpty(rarity) ? "—" : char.ToUpper(rarity[0]) + rarity[1..];
                rows.Add([status, rarityDisplay, charId, snake, title, CleanDesc(desc), url]);
            }

        Console.WriteLine($"  {rows.Count} rows ({skippedFinal} FINAL skipped).");
        if (missingLoc.Count > 0)
        {
            Console.WriteLine($"  [warn] {missingLoc.Count} with no localization:");
            foreach (var k in missingLoc.Take(10)) Console.WriteLine($"    - {k}");
            if (missingLoc.Count > 10) Console.WriteLine($"    ... and {missingLoc.Count - 10} more.");
        }

        return rows;
    }

    // ── Sheets Sync ───────────────────────────────────────────────────────────

    private void SyncSheet(SheetsService svc, Dictionary<string, string> cache,
        string name, List<List<string>> rows, Color tabColor)
    {
        var hash = RowsHash(rows);
        if (cache.TryGetValue(name, out var cached) && cached == hash)
        {
            Console.WriteLine($"  [{name}] No changes, skipping.");
            return;
        }

        Console.WriteLine($"  [{name}] Updating {rows.Count} rows...");

        // Find or create sheet
        var spreadsheet = svc.Spreadsheets.Get(_sheetId).Execute();
        var existing = spreadsheet.Sheets.FirstOrDefault(s => s.Properties.Title == name);
        int sheetId;

        if (existing is not null)
        {
            sheetId = (int)existing.Properties.SheetId!;
        }
        else
        {
            var resp = svc.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest
            {
                Requests =
                [
                    new Request { AddSheet = new AddSheetRequest { Properties = new SheetProperties { Title = name } } }
                ]
            }, _sheetId).Execute();
            sheetId = (int)resp.Replies[0].AddSheet.Properties.SheetId!;
        }

        // Clear and write
        svc.Spreadsheets.Values.Clear(new ClearValuesRequest(), _sheetId, name).Execute();

        var data = rows.Count > 0
            ? new[] { Header.ToList() }.Concat(rows).ToList()
            : [Header.ToList(), ["All caught up!"]];

        var writeReq = svc.Spreadsheets.Values.Update(
            new ValueRange { Values = data.Select(r => (IList<object>)r.Cast<object>().ToList()).ToList() },
            _sheetId, $"{name}!A1");
        writeReq.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        writeReq.Execute();

        if (rows.Count == 0)
        {
            cache[name] = hash;
            return;
        }

        // Dimension + tab color
        svc.Spreadsheets.BatchUpdate(new BatchUpdateSpreadsheetRequest
        {
            Requests =
            [
                DimRequest(sheetId, "ROWS", 1, rows.Count + 1, RowHeightPx),
                DimRequest(sheetId, "COLUMNS", 6, 7, ImgColPx),
                TabColorRequest(sheetId, tabColor)
            ]
        }, _sheetId).Execute();

        // Row colors — batch in chunks of 500 to stay under API limits
        var cellRequests = new List<Request>();
        for (var i = 0; i < rows.Count; i++)
        {
            var row = i + 2;
            var statusBg = rows[i][0] switch
            {
                "MISSING" => ColMissing,
                "PLACEHOLDER" => ColPlaceholder,
                _ => ColFinal
            };
            cellRequests.Add(RepeatCell(sheetId, row - 1, row, 0, 1, CellFmt(statusBg, "MIDDLE", "CENTER")));
            cellRequests.Add(RepeatCell(sheetId, row - 1, row, 1, 6, CellFmt(ColWhite, "MIDDLE")));
            cellRequests.Add(RepeatCell(sheetId, row - 1, row, 6, 7, CellFmt(ColWhite, "MIDDLE")));
        }

        foreach (var chunk in cellRequests.Chunk(500))
            svc.Spreadsheets.BatchUpdate(
                new BatchUpdateSpreadsheetRequest { Requests = [..chunk] }, _sheetId).Execute();

        cache[name] = hash;
        Console.WriteLine($"  [{name}] Done.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static string ToSnake(string name)
    {
        return Regex.Replace(name, @"(?<!^)(?=[A-Z])", "_").ToLower();
    }

    private static string Normalize(string name, bool isPower)
    {
        var snake = ToSnake(name);
        if (isPower) snake = Regex.Replace(snake, "_power$", "");
        return snake.Replace("_", "");
    }

    private static string CleanDesc(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        s = Regex.Replace(s, @"\{[^}]+\}", "");
        s = Regex.Replace(s, @"\[#?[0-9a-fA-F]{6}\]|\[/?b\]|\[nl\]", " ", RegexOptions.IgnoreCase);
        s = Regex.Replace(s, @"\[/?[^\]]+\]", "");
        return s.Replace('\n', ' ').Trim();
    }

    private static string RowsHash(List<List<string>> rows)
    {
        return Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(rows)))).ToLower();
    }

    private static Dictionary<string, string> LoadDict(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"  [cache] not found: {path}");
            return new Dictionary<string, string>();
        }

        Console.WriteLine($"  [cache] loaded: {path}");
        return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(path)) ??
               new Dictionary<string, string>();
    }

    private static Color Rgb(float r, float g, float b)
    {
        return new Color { Red = r, Green = g, Blue = b };
    }

    private static Request DimRequest(int sheetId, string dim, int start, int end, int px)
    {
        return new Request
        {
            UpdateDimensionProperties = new UpdateDimensionPropertiesRequest
            {
                Range = new DimensionRange { SheetId = sheetId, Dimension = dim, StartIndex = start, EndIndex = end },
                Properties = new DimensionProperties { PixelSize = px },
                Fields = "pixelSize"
            }
        };
    }

    private static Request TabColorRequest(int sheetId, Color color)
    {
        return new Request
        {
            UpdateSheetProperties = new UpdateSheetPropertiesRequest
            {
                Properties = new SheetProperties
                    { SheetId = sheetId, TabColorStyle = new ColorStyle { RgbColor = color } },
                Fields = "tabColorStyle"
            }
        };
    }

    private static Request RepeatCell(int sheetId, int r0, int r1, int c0, int c1, CellFormat fmt)
    {
        return new Request
        {
            RepeatCell = new RepeatCellRequest
            {
                Range = new GridRange
                {
                    SheetId = sheetId, StartRowIndex = r0, EndRowIndex = r1, StartColumnIndex = c0, EndColumnIndex = c1
                },
                Cell = new CellData { UserEnteredFormat = fmt },
                Fields = "userEnteredFormat(backgroundColor,verticalAlignment,horizontalAlignment)"
            }
        };
    }

    private static CellFormat CellFmt(Color bg, string vAlign, string? hAlign = null)
    {
        return new CellFormat
        {
            BackgroundColor = bg,
            VerticalAlignment = vAlign,
            HorizontalAlignment = hAlign
        };
    }

    // ── Character Discovery ───────────────────────────────────────────────────

    private record CharInfo(string CodeDir, string Project, string ProjectPath);

    // ── Image Indexing ────────────────────────────────────────────────────────

    private record ImageEntry(string Status, string Subdir, string RelFromSubdir);

    // ── Code Collection ───────────────────────────────────────────────────────

    private record CodeEntry(string Snake, string Rarity, string LocPrefix);
}