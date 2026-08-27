using Downfall.DownfallCode.Voting;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;

namespace Downfall.TestCode;


[ModInitializer(nameof(Initialize))]
public static class TestMainFile
{
    public const string ModId = "Test"; //At the moment, this is used only for the Logger and harmony names.

    public static Logger Logger { get; } =
        new(ModId, LogType.Generic);

    public static void Initialize()
    {
        MainMenuButtonRegistry.Register(new MainMenuButtonRegistry.Entry
        {
            Label = "Auto Slay",
            IsVisible = () => true,
            SubmenuType = null,
            CreateSubmenu = null,
            OnPress = _ =>
            {
                var cts = new CancellationTokenSource(TimeSpan.FromMinutes(10));
                var harness = new CardTestRunner();

                TaskHelper.RunSafely(harness.RunAllTestsAsync(SeedHelper.GetRandomSeed(), cts.Token));
            }
        });
    }

}