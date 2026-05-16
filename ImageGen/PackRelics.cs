using System.Text.Json.Nodes;
using ImageGen.Pipeline;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace ImageGen;

public class PackRelics(string scriptDir, bool force)
    : ImagePipeline(scriptDir, ".relics_cache.json", force)
{
    private const int ImgSize = 93;
    private const int Inset = 4;
    private const int RegionSize = 85;
    private const int OutlineRadius = 10;
    private const float OutlineSigma = 0.5f;

    private static readonly Rectangle CropBox = new(56, 56, 144, 144); // x,y,w,h (200-56=144)

    protected override IEnumerable<string> DiscoverCharacters()
    {
        var relicsRoot = Path.Join(ImagesDir, "relics");
        if (!Directory.Exists(relicsRoot)) return [];
        return Directory.EnumerateDirectories(relicsRoot)
            .Select(d => Path.GetFileName(d)!.ToLower());
    }

    protected override JsonObject ProcessCharacter(string charId, string charProj, JsonObject charCache)
    {
        var inputDir = Path.Join(ImagesDir, "relics", charId);
        var outAtlases = Path.Join(Parent, charProj, "images", "atlases");
        var outRelics = Path.Join(Parent, charProj, "images", "relics");
        var outTres = Path.Join(outAtlases, "relic_atlas.sprites");
        var resBase = $"res://{charProj}/images/atlases";
        var atlasRes = $"{resBase}/relic_atlas.png";
        var outlineRes = $"{resBase}/relic_outline_atlas.png";

        foreach (var d in new[] { outAtlases, outRelics, outTres })
            Directory.CreateDirectory(d);

        var pngFiles = Directory.EnumerateFiles(inputDir, "*.png").Order().Select(Path.GetFileName).ToList();
        var currentHashes = pngFiles.ToDictionary(f => f!, f => Utils.FileHash(Path.Join(inputDir, f!)));

        var outputsExist =
            File.Exists(Path.Join(outAtlases, "relic_atlas.png")) &&
            File.Exists(Path.Join(outAtlases, "relic_outline_atlas.png"));

        if (outputsExist && charCache["input_hashes"] is JsonObject cachedHashes &&
            currentHashes.All(kv => cachedHashes[kv.Key]?.GetValue<string>() == kv.Value))
        {
            Console.WriteLine("  nothing changed, skipping");
            return charCache;
        }

        var prevHashes = charCache["input_hashes"]?.AsObject();
        var changed = currentHashes.Where(kv => prevHashes?[kv.Key]?.GetValue<string>() != kv.Value)
            .Select(kv => kv.Key).ToHashSet();

        var entries =
            new List<(string Stem, Image<Rgba32> Big, Image<Rgba32> OutlineDs, Image<Rgba32> ImageDs, bool IsChanged
                )>();

        foreach (var file in pngFiles)
        {
            var stem = Path.GetFileNameWithoutExtension(file!);
            var (big, outlineDs, imageDs) = ProcessRelic(Path.Join(inputDir, file!));
            var isChanged = changed.Contains(file!);
            entries.Add((stem, big, outlineDs, imageDs, isChanged));
            Console.WriteLine($"  processed: {file}" + (isChanged ? " (changed)" : ""));
        }

        if (entries.Count == 0)
        {
            Console.WriteLine("  no images");
            return charCache;
        }

        var estWidth =
            Math.Max(Utils.NextPowerOfTwo((int)(Math.Sqrt(entries.Count * (ImgSize + 1.0) * (ImgSize + 1.0)) * 1.2)),
                ImgSize + 1);
        var packer = new ShelfPacker(estWidth);
        var places = entries.Select(_ => packer.Pack(ImgSize, ImgSize)).ToList();

        int aw = Utils.NextPowerOfTwo(packer.CanvasSize.Width), ah = packer.CanvasSize.Height;
        using var atlas = new Image<Rgba32>(aw, ah);
        using var outlineAtlas = new Image<Rgba32>(aw, ah);

        for (var i = 0; i < entries.Count; i++)
        {
            var (stem, big, outlineDs, imageDs, isChanged) = entries[i];
            var (x, y) = places[i];
            atlas.Mutate(ctx => ctx.DrawImage(imageDs, new Point(x, y), 1f));
            outlineAtlas.Mutate(ctx => ctx.DrawImage(outlineDs, new Point(x, y), 1f));

            if (isChanged && Utils.SaveImageIfChanged(big, Path.Join(outRelics, $"{stem}.png")))
                Console.WriteLine($"  updated big: {stem}");
        }

        Utils.SaveImageIfChanged(atlas, Path.Join(outAtlases, "relic_atlas.png"));
        Utils.SaveImageIfChanged(outlineAtlas, Path.Join(outAtlases, "relic_outline_atlas.png"));

        for (var i = 0; i < entries.Count; i++)
        {
            var (stem, _, _, _, _) = entries[i];
            var (x, y) = places[i];
            Utils.WriteTres(Path.Join(outTres, $"{stem}.tres"), atlasRes, x + Inset, y + Inset, RegionSize, RegionSize,
                $"{charId}_{stem}");
            Utils.WriteTres(Path.Join(outTres, $"{stem}_outline.tres"), outlineRes, x + Inset, y + Inset, RegionSize,
                RegionSize, $"{charId}_{stem}_outline");
        }

        foreach (var (_, big, outlineDs, imageDs, _) in entries)
        {
            big.Dispose();
            outlineDs.Dispose();
            imageDs.Dispose();
        }

        charCache["input_hashes"] = new JsonObject(
            currentHashes.Select(kv => KeyValuePair.Create<string, JsonNode?>(kv.Key, kv.Value)));

        return charCache;
    }

    private (Image<Rgba32> Big, Image<Rgba32> OutlineDs, Image<Rgba32> ImageDs) ProcessRelic(string path)
    {
        using var raw = Image.Load<Rgba32>(path);
        using var cropped = raw.Clone(ctx => ctx.Crop(CropBox));
        using var upscaled = cropped.Clone(ctx => ctx.Resize(256, 256, KnownResamplers.Lanczos3));

        var big = Outline.ApplyOutline(upscaled, OutlineRadius, OutlineSigma);
        var outlineFull = Outline.WhiteOutlineImage(upscaled, OutlineRadius, OutlineSigma);
        var outlineDs = outlineFull.Clone(ctx => ctx.Resize(ImgSize, ImgSize, KnownResamplers.Lanczos3));
        var imageDs = cropped.Clone(ctx => ctx.Resize(ImgSize, ImgSize, KnownResamplers.Lanczos3));

        outlineFull.Dispose();
        return (big, outlineDs, imageDs);
    }
}