namespace Downfall.DownfallCode.Audio;

internal static class FmodStudioGuidInterop
{
    internal static bool TryNormalizeForAddon(string raw, out string bracedLowercase)
    {
        bracedLowercase = string.Empty;
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        var trimmed = raw.Trim();
        if (trimmed is ['{', _, ..] && trimmed[^1] == '}')
            trimmed = trimmed[1..^1].Trim();

        if (!Guid.TryParse(trimmed, out var guid))
            return false;

        bracedLowercase = guid.ToString("B");
        return true;
    }
}