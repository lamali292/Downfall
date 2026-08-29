using HarmonyLib;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Runs;
using Downfall.DownfallCode.Config;

namespace Downfall.DownfallCode.Patches.KaleidoscopePatch;

[HarmonyPatch]
public static class RunConfigSyncHook
{
    private static readonly MessageHandlerDelegate<PrismaticConfigMessage> Handler =
        PrismaticModeConfigSync.OnConfig;

    // 1. Net service exists here → register the receive handler + wire re-broadcast.
    //    Runs on EVERY setup path (new/saved/SP/MP) because they all call InitializeShared.
    [HarmonyPatch(typeof(RunManager), "InitializeShared")]
    [HarmonyPostfix]
    public static void OnInitShared(RunManager __instance)
    {
        var net = __instance.NetService;
        net.RegisterMessageHandler(Handler);

        PrismaticModeConfigSync.SetRebroadcast(() =>
        {
            if (net.Type != NetGameType.Singleplayer)
                net.SendMessage(new PrismaticConfigMessage
                {
                    OwnerNetId = net.NetId,
                    PrismaticMode = DownfallConfig.PrismaticOption,
                });
        });
    }

    // 2. Launch = universal run-start. LocalContext.NetId is set at its top,
    //    so this is the one place capture works on new, saved, AND resumed runs.
    [HarmonyPatch(typeof(RunManager), nameof(RunManager.Launch))]
    [HarmonyPostfix]
    public static void OnLaunch(RunManager __instance)
    {
        PrismaticModeConfigSync.Reset();        // fresh slate for this run
        PrismaticModeConfigSync.CaptureLocal(); // freeze my own config (NetId now set)

        var net = __instance.NetService;
        if (net.Type != NetGameType.Singleplayer)
            net.SendMessage(new PrismaticConfigMessage      // tell peers my frozen value
            {
                OwnerNetId = net.NetId,
                PrismaticMode = DownfallConfig.PrismaticOption,
            });
    }

    // 3. Teardown → unregister. Don't clear ByOwner; Launch resets it next run.
    [HarmonyPatch(typeof(RunManager), nameof(RunManager.CleanUp))]
    [HarmonyPostfix]
    public static void OnCleanUp(RunManager __instance)
    {
        __instance.NetService?.UnregisterMessageHandler(Handler);
        PrismaticModeConfigSync.SetRebroadcast(null);
    }
}