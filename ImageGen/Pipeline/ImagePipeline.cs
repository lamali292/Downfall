using System.Text.Json;
using System.Text.Json.Nodes;

namespace ImageGen.Pipeline;

public abstract class ImagePipeline
{
    private readonly JsonObject _cache;

    private readonly string _cacheFile;
    protected readonly bool Force;
    protected readonly string ImagesDir;
    protected readonly string Parent;

    protected ImagePipeline(string scriptDir, string cacheFileName, bool force)
    {
        ImagesDir = Path.Join(scriptDir, "images");
        Parent = Path.GetFullPath(Path.Join(scriptDir, ".."));
        Force = force;
        _cacheFile = Path.Join(scriptDir, cacheFileName);
        _cache = File.Exists(_cacheFile)
            ? JsonNode.Parse(File.ReadAllText(_cacheFile))!.AsObject()
            : new JsonObject();
    }

    // ── Override in subclasses ────────────────────────────────────────────────

    protected abstract IEnumerable<string> DiscoverCharacters();

    protected abstract JsonObject ProcessCharacter(string charId, string charProj, JsonObject charCache);

    // ── Entry point ───────────────────────────────────────────────────────────

    public void Run()
    {
        foreach (var charId in DiscoverCharacters().Order())
        {
            var charProj = FindCharProj(charId);
            if (charProj is null)
            {
                Console.WriteLine($"No project folder for {charId}, skipping");
                continue;
            }

            Console.WriteLine($"\n=== {charId} -> {charProj} ===");
            _cache[charId] = ProcessCharacter(charId, charProj, _cache[charId]?.AsObject() ?? new JsonObject());
        }

        File.WriteAllText(_cacheFile, _cache.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));
        Console.WriteLine("\nDone!");
    }

    // ── Shared helpers ────────────────────────────────────────────────────────

    protected IEnumerable<string> DiscoverFromSubdirs(IEnumerable<string> subdirs)
    {
        return subdirs
            .Select(sub => Path.Join(ImagesDir, sub))
            .Where(Directory.Exists)
            .SelectMany(Directory.EnumerateDirectories)
            .Select(d => Path.GetFileName(d).ToLower())
            .ToHashSet();
    }

    private string? FindCharProj(string charId)
    {
        if (charId == "downfall") return "Downfall";
        return Directory.EnumerateDirectories(Parent)
            .Select(Path.GetFileName)
            .FirstOrDefault(e =>
                e!.ToLower() == charId &&
                !e.EndsWith("Code", StringComparison.OrdinalIgnoreCase));
    }
}