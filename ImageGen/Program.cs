using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using ImageGen;

static string ProjectDir([CallerFilePath] string file = "")
{
    return Path.GetDirectoryName(file)!;
}

var scriptDir = ProjectDir();

if (args.Contains("--sync-cards"))
{
    const string sheetId = "1adgDbqi4A7oHqtAb2klUFrUsl4-TQR_AIWqDdDQUQ1g";
    var localProps = Path.Join(scriptDir, "..", "local.props");
    var steamPath = MyRegex().Match(File.ReadAllText(localProps)).Groups[1].Value.Trim();
    new SyncSheets(scriptDir, sheetId).Run();
    var cardsJson = Path.Join(steamPath, "common", "Slay the Spire 2", "export", "cards.json");
    new SyncCardSheets(scriptDir).Run(cardsJson, sheetId);
}
else
{
    var force = args.Contains("--repack");
    new PackCards(scriptDir, force).Run();
    new PackPowers(scriptDir, force).Run();
    new PackRelics(scriptDir, force).Run();
    new PackPotions(scriptDir, force).Run();
    new PackEnchantments(scriptDir, force).Run();
}


internal partial class Program
{
    [GeneratedRegex(@"<SteamLibraryPath>(.*?)</SteamLibraryPath>")]
    private static partial Regex MyRegex();
}