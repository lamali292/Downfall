using System.Text.Json.Nodes;
using ImageGen.Pipeline;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGen;

public class PackEnchantments(string scriptDir, bool force)
    : ImagePipeline(scriptDir, ".enchantments_cache.json", force)
{
    private const int OutputSize = 64;
    private const float ContentScale = 0.9f;

    private const int OutlineRadius = 3;
    private const float OutlineSigma = 0.1f;

    private static readonly string[] InputSubdirs = ["enchantments", "enchantments_beta"];

    protected override IEnumerable<string> DiscoverCharacters()
    {
        return DiscoverFromSubdirs(InputSubdirs);
    }

    protected override JsonObject ProcessCharacter(string charId, string charProj, JsonObject charCache)
    {
        var outDir = Path.Join(Parent, charProj, "images", "enchantments");
        Directory.CreateDirectory(outDir);

        // Collect input files — subdirs in priority order, first-seen filename wins
        var seen = new HashSet<string>();
        var inputFiles = (from sub in InputSubdirs
            select Path.Join(ImagesDir, sub, charId)
            into d
            where Directory.Exists(d)
            from file in Directory.EnumerateFiles(d, "*.png").Order()
            where seen.Add(Path.GetFileName(file))
            select file).ToList();

        if (inputFiles.Count == 0)
        {
            Console.WriteLine("  no images found");
            return charCache;
        }

        var currentHashes = inputFiles.ToDictionary(f => Path.GetFileName(f)!, Utils.FileHash);
        var allOutputsExist = inputFiles.All(f =>
            File.Exists(Path.Join(outDir, Path.GetFileName(f))));

        if (!Force && allOutputsExist && charCache["input_hashes"] is JsonObject cachedHashes &&
            currentHashes.All(kv => cachedHashes[kv.Key]?.GetValue<string>() == kv.Value))
        {
            Console.WriteLine("  nothing changed, skipping");
            return charCache;
        }

        var updated = 0;
        foreach (var file in inputFiles)
        {
            var fileName = Path.GetFileName(file);
            using var raw = Image.Load<Rgba32>(file);
            using var outlined = Outline.ApplyOutline(ScaleCentered(raw, OutputSize), OutlineRadius);

            if (Utils.SaveImageIfChanged(outlined, Path.Join(outDir, fileName)))
            {
                Console.WriteLine($"  updated: {fileName}");
                updated++;
            }
        }

        Console.WriteLine($"  {updated} enchantments updated ({inputFiles.Count} total)");

        charCache["input_hashes"] = new JsonObject(
            currentHashes.Select(kv => KeyValuePair.Create<string, JsonNode?>(kv.Key, kv.Value)));

        return charCache;
    }

    private static Image<Rgba32> ScaleCentered(Image<Rgba32> src, int full)
    {
        var inner = Math.Max(1, (int)MathF.Round(full * ContentScale));
        return src.Clone(ctx => ctx
            .Resize(inner, inner, KnownResamplers.Lanczos3)
            .Resize(new ResizeOptions
            {
                Size = new Size(full, full),
                Mode = ResizeMode.BoxPad,
                Position = AnchorPositionMode.Center
            }));
    }
}