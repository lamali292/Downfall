namespace SlimeBoss.SlimeBossCode.Extensions;

public static class StringExtensions
{
    extension(string path)
    {
        public string SlimeScenePath()
        {
            return Downfall.DownfallCode.Extensions.StringExtensions.ScenePath(SlimeBossMainFile.ModId, "slimes", path);
        }
    }
}