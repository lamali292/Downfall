using FileAccess = Godot.FileAccess;

namespace Downfall.DownfallCode.Audio;

internal static class FmodGuids
{
    private static Dictionary<string, string> _map = new(StringComparer.Ordinal);

    private static Dictionary<string, string> Parse(string text)
    {
        var map = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var rawLine in text.Split('\n'))
        {
            var line = rawLine.Trim();
            if (line.Length == 0 || line[0] != '{')
                continue;

            var close = line.IndexOf('}');
            if (close <= 1 || !Guid.TryParse(line.AsSpan(1, close - 1), out var guid))
                continue;

            var path = line[(close + 1)..].TrimStart();
            if (path.StartsWith("event:", StringComparison.Ordinal))
                map[path] = guid.ToString("B");
        }

        return map;
    }
    
    public static bool LoadFile(string resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath) || !FileAccess.FileExists(resourcePath))
        {
            DownfallMainFile.Logger.Warn($"[Audio] guids: missing file: {resourcePath}");
            return false;
        }

        try
        {
            using var file = FileAccess.Open(resourcePath, FileAccess.ModeFlags.Read);
            if (file is null)
                return false;

            Merge(Parse(file.GetAsText()));
            return true;
        }
        catch (Exception ex)
        {
            DownfallMainFile.Logger.Error($"[Audio] guids '{resourcePath}': {ex.Message}");
            return false;
        }
    }

    public static bool TryGetGuid(string eventPath, out string guid)
    {
        guid = string.Empty;
        return !string.IsNullOrEmpty(eventPath) && Volatile.Read(ref _map).TryGetValue(eventPath, out guid!);
    }

    private static void Merge(Dictionary<string, string> parsed)
    {
        if (parsed.Count == 0)
            return;

        var next = new Dictionary<string, string>(Volatile.Read(ref _map), StringComparer.Ordinal);
        foreach (var (path, guid) in parsed)
            next[path] = guid;

        Volatile.Write(ref _map, next);
    }
}