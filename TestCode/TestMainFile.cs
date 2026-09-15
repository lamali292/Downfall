using Downfall.DownfallCode.Voting;
using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using Logger = MegaCrit.Sts2.Core.Logging.Logger;

namespace Downfall.TestCode;


[ModInitializer(nameof(Initialize))]
public static class TestMainFile
{
    public const string ModId = "Test"; //At the moment, this is used only for the Logger and harmony names.

    /// Set to "1" to run the tests automatically once the main menu is up, then quit the game.
    /// Exit code is 0 when everything passed, 1 otherwise. See build/test.ps1.
    private const string EnvRunTests = "DOWNFALL_RUN_TESTS";

    /// Optional substring filter on "Type.Method" (case-insensitive).
    private const string EnvFilter = "DOWNFALL_TEST_FILTER";

    /// Where the JSON result is written. Defaults to user://downfall_tests.json.
    private const string EnvOutput = "DOWNFALL_TEST_OUTPUT";

    private static bool _autoRunStarted;

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        MainMenuButtonRegistry.Register(new MainMenuButtonRegistry.Entry
        {
            Label = "Unit Test",
            IsVisible = () => true,
            SubmenuType = null,
            CreateSubmenu = null,
            OnPress = _ => TaskHelper.RunSafely(RunTests(System.Environment.GetEnvironmentVariable(EnvFilter)))
        });

        if (System.Environment.GetEnvironmentVariable(EnvRunTests) == "1")
            MainMenuButtonRegistry.MainMenuReady += AutoRun;
    }

    private static void AutoRun()
    {
        if (_autoRunStarted) return;
        _autoRunStarted = true;
        MainMenuButtonRegistry.MainMenuReady -= AutoRun;
        // Let the menu finish its first frame before we tear the scene tree around.
        Callable.From(() => TaskHelper.RunSafely(AutoRunAsync())).CallDeferred();
    }

    private static async Task AutoRunAsync()
    {
        var exitCode = 1;
        try
        {
            var result = await RunTests(System.Environment.GetEnvironmentVariable(EnvFilter));
            var output = System.Environment.GetEnvironmentVariable(EnvOutput);
            if (string.IsNullOrEmpty(output))
                output = ProjectSettings.GlobalizePath("user://downfall_tests.json");
            result.WriteJson(output);
            Logger.Info($"Test results written to {output}");
            exitCode = result.Success ? 0 : 1;
        }
        catch (Exception e)
        {
            Logger.Error($"Test run crashed: {e}");
        }
        finally
        {
            var tree = (SceneTree)Engine.GetMainLoop();
            tree.Quit(exitCode);
        }
    }

    private static Task<TestRunResult> RunTests(string? filter)
    {
        var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
        var harness = new CardTestRunner();
        return harness.RunAllTestsAsync(SeedHelper.GetRandomSeed(), cts.Token, filter);
    }
}
