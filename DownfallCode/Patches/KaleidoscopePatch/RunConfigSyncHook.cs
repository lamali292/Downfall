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
    
    [HarmonyPatch(typeof(RunManager), nameof(RunManager.Launch))]
    [HarmonyPostfix]
    public static void OnLaunch(RunManager __instance)
    {
        PrismaticModeConfigSync.Reset();      
        PrismaticModeConfigSync.CaptureLocal(); 

        var net = __instance.NetService;
        if (net.Type != NetGameType.Singleplayer)
            net.SendMessage(new PrismaticConfigMessage     
            {
                OwnerNetId = net.NetId,
                PrismaticMode = DownfallConfig.PrismaticOption,
            });
    }
    
    [HarmonyPatch(typeof(RunManager), nameof(RunManager.CleanUp))]
    [HarmonyPostfix]
    public static void OnCleanUp(RunManager __instance)
    {
        __instance.NetService?.UnregisterMessageHandler(Handler);
        PrismaticModeConfigSync.SetRebroadcast(null);
    }
}