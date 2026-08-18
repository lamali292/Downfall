namespace Guardian.GuardianCode.Extensions;

internal static class StringExtensions
{
    extension(string path)
    {
        public string GemPath()
        {
            return Path.Join(GuardianMainFile.ModId, "images", "gems", path);
        }
    }
   
}