namespace SlimeBoss.SlimeBossCode.Extensions;

public static class StringExtensions
{
    public static string SlimeScenePath(this string path)
    {
        return Downfall.DownfallCode.Extensions.StringExtensions.ScenePath(SlimeBossMainFile.ModId, "slimes", path);
    }
}