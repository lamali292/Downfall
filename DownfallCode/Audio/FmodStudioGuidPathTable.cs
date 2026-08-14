using FileAccess = Godot.FileAccess;

namespace Downfall.DownfallCode.Audio;

internal static class FmodStudioGuidPathTable
{
    private static readonly Lock Gate = new();
    private static Dictionary<string, string> _eventPathToGuid = [];
    
    internal static bool TryLoadFromResourceFile(string resourcePath, out int parsedEventMappings)
    {
        parsedEventMappings = 0;
        if (string.IsNullOrWhiteSpace(resourcePath) || !FileAccess.FileExists(resourcePath))
            return false;

        try
        {
            using var file = FileAccess.Open(resourcePath, FileAccess.ModeFlags.Read);
            if (file is null)
                return false;

            parsedEventMappings = ParseAndMerge(file.GetAsText(), resourcePath);
            return true;
        }
        catch (Exception ex)
        {
            DownfallMainFile.Logger.Error($"[Audio] Failed to load FMOD GUID mappings from '{resourcePath}': {ex.Message}");
            return false;
        }
    }

    private static int ParseAndMerge(string text, string? sourceLabel = null)
    {
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var prefix = string.IsNullOrEmpty(sourceLabel) ? "[Audio] guids.txt" : $"[Audio] guids.txt ({sourceLabel})";
        var parsed = 0;

        lock (Gate)
        {
            var next = new Dictionary<string, string>(_eventPathToGuid, StringComparer.Ordinal);

            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i].Trim();
                if (line.Length == 0 || line[0] == '#')
                    continue;

                var close = line.IndexOf('}', StringComparison.Ordinal);
                if (line[0] != '{' || close <= 1)
                {
                    DownfallMainFile.Logger.Warn($"{prefix} line {i + 1}: expected '{{guid}} …', skipped.");
                    continue;
                }

                var guidFragment = line.AsSpan(1, close - 1).Trim();
                if (guidFragment.IsEmpty)
                    continue;

                if (!Guid.TryParse(guidFragment, out var guid))
                {
                    DownfallMainFile.Logger.Warn($"{prefix} line {i + 1}: invalid GUID, skipped.");
                    continue;
                }

                var pathPart = close + 1 < line.Length ? line[(close + 1)..].TrimStart() : string.Empty;
                if (!pathPart.StartsWith("event:", StringComparison.Ordinal))
                    continue;

                next[pathPart] = guid.ToString("B");
                parsed++;
            }

            _eventPathToGuid = next;
        }

        return parsed;
    }
    
    internal static bool TryGetGuidForEventPath(string eventPath, out string guid)
    {
        guid = string.Empty;
        if (string.IsNullOrEmpty(eventPath))
            return false;

        lock (Gate)
        {
            if (!_eventPathToGuid.TryGetValue(eventPath, out var v) || v is null)
                return false;
            guid = v;
            return true;
        }
    }
}