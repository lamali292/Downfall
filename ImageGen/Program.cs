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
    var propsText = File.ReadAllText(localProps);
    var branch = ReadProp(propsText, "Sts2Branch") is { Length: > 0 } b ? b : "beta";
    var gamePath = ReadProp(propsText, branch == "public" ? "Sts2PublicPath" : "Sts2BetaPath");
    new SyncSheets(scriptDir, sheetId).Run();
    var cardsJson = Path.Join(gamePath, "export", "cards.json");
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


static string ReadProp(string propsText, string name) =>
    Regex.Match(propsText, $"<{name}>(.*?)</{name}>").Groups[1].Value.Trim();