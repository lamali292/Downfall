using MegaCrit.Sts2.Core.Platform.Steam;
using Steamworks;
using Downfall.DownfallCode.Voting;

namespace Downfall.DownfallCode;

/// <summary>
/// Reports playtime to Steam for the Downfall Workshop item, so it shows up in the item's
/// playtime stats. Only this item's id is ever passed in: <see cref="SteamUGC.StartPlaytimeTracking"/>
/// is per-item, so it never affects tracking other mods started for their own items. Steam stops
/// tracking by itself when the game exits, so there is deliberately no stop call
/// (<c>StopPlaytimeTrackingForAllItems</c> would also stop other mods' items).
/// </summary>
public static class WorkshopPlaytime
{
    private const ulong DownfallWorkshopId = 3747508091;

    private static bool _started;

    /// <summary>
    /// Steam is initialized by <c>NGame</c> after mods load, so this has to wait for the main menu
    /// rather than run from <c>Initialize()</c>.
    /// </summary>
    public static void Register() => MainMenuButtonRegistry.MainMenuReady += Start;

    private static void Start()
    {
        if (_started || !SteamInitializer.Initialized) return;
        _started = true;
        // Runs inside the MainMenuReady event: an exception here would stop later subscribers.
        try
        {
            SteamUGC.StartPlaytimeTracking([new PublishedFileId_t(DownfallWorkshopId)], 1);
        }
        catch (Exception ex)
        {
            DownfallMainFile.Logger.Warn($"[Steam] playtime tracking failed: {ex.Message}");
        }
    }
}
