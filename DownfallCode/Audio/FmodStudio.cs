using Godot;

namespace Downfall.DownfallCode.Audio;

public static class FmodStudio
{
    private static readonly Lock Gate = new();
    private static readonly List<string> PendingBanks = [];
    private static readonly List<string> PendingGuidFiles = [];
    private static readonly List<GodotObject> LoadedBanks = [];
    private static bool _ready;

    private static readonly StringName PlayOneShot = new("play_one_shot_using_guid");
    private static readonly StringName WaitForAllLoads = new("wait_for_all_loads");

    public static void RegisterBank(string resourcePath)
    {
        Enqueue(PendingBanks, resourcePath);
    }

    public static void RegisterGuidMappings(string resourcePath)
    {
        Enqueue(PendingGuidFiles, resourcePath);
    }

    public static void OnDeferredInitializationCompleted()
    {
        lock (Gate)
        {
            _ready = true;
        }

        Flush();
    }

    public static bool TryPlayEvent(string eventPath)
    {
        if (!FmodGuids.TryGetGuid(eventPath, out var guid))
            return false;

        FmodServer.Call(PlayOneShot, guid, 1f);
        return true;
    }

    private static void Enqueue(List<string> target, string resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath))
            return;

        bool flushNow;
        lock (Gate)
        {
            target.Add(resourcePath.Trim());
            flushNow = _ready;
        }

        if (flushNow)
            Flush();
    }

    private static void Flush()
    {
        if (FmodServer.Get() is null)
            return;

        List<string> banks, guidFiles;
        lock (Gate)
        {
            if (PendingBanks.Count == 0 && PendingGuidFiles.Count == 0)
                return;

            banks = [.. PendingBanks];
            guidFiles = [.. PendingGuidFiles];
            PendingBanks.Clear();
            PendingGuidFiles.Clear();
        }

        foreach (var path in guidFiles)
            FmodGuids.LoadFile(path);

        foreach (var path in banks)
            if (FmodServer.LoadBank(path) is { } bank)
                lock (Gate)
                {
                    LoadedBanks.Add(bank);
                }

        if (banks.Count > 0)
            FmodServer.Call(WaitForAllLoads);

        DownfallMainFile.Logger.Info(
            $"[Audio] FMOD flush: {banks.Count} bank(s), {guidFiles.Count} guid file(s).");
    }
}