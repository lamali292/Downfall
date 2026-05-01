namespace Guardian.GuardianCode.Extensions;

internal static class StringExtensions
{
    public static string GemPath(this string path)
    {
        return Path.Join(GuardianMainFile.ModId, "images", "gems", path);
    }
}