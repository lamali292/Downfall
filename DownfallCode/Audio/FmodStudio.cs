using Godot;
using FileAccess = Godot.FileAccess;

namespace Downfall.DownfallCode.Audio;

public static class FmodStudio
{
    private static readonly StringName LoadBank = new("load_bank");
    private static readonly StringName PlayOneShotUsingGuid = new("play_one_shot_using_guid");
    private static readonly StringName WaitForAllLoads = new("wait_for_all_loads");

    private static readonly Lock QueueGate = new();
    private static readonly Lock FlushGate = new();
    private static readonly Lock BankPinsGate = new();

    private static readonly HashSet<string> PendingBanks = new(StringComparer.Ordinal);
    private static readonly HashSet<string> PendingGuidFiles = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, GodotObject> BankPins = [];

    private static bool _initialized;

    private static readonly StringName[] GuidInjectCandidates =
    [
        new("register_guid_path_mappings_from_file"),
        new("inject_guid_mappings_from_file"),
        new("register_strings_from_guid_file"),
        new("load_guid_mapping_file"),
    ];

    public static void RegisterBank(string resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath))
            return;

        bool flushNow;
        lock (QueueGate)
        {
            PendingBanks.Add(resourcePath.Trim());
            flushNow = _initialized;
        }

        if (flushNow)
            Flush();
    }

    public static void RegisterGuidMappings(string guidMapResourcePath)
    {
        if (string.IsNullOrWhiteSpace(guidMapResourcePath))
            return;

        bool flushNow;
        lock (QueueGate)
        {
            PendingGuidFiles.Add(guidMapResourcePath.Trim());
            flushNow = _initialized;
        }

        if (flushNow)
            Flush();
    }

    public static void OnDeferredInitializationCompleted()
    {
        lock (QueueGate)
            _initialized = true;

        Flush();
    }

    public static bool TryPlayEvent(string eventPath)
    {
        if (!FmodStudioGuidPathTable.TryGetGuidForEventPath(eventPath, out var guid)
            || !TryNormalizeGuid(guid, out var normalized))
            return false;

        FmodStudioGateway.TryCall(PlayOneShotUsingGuid, normalized, 1f);
        return true;
    }

    private static void Flush()
    {
        lock (FlushGate)
            FlushCore();
    }

    private static void FlushCore()
    {
        if (FmodStudioGateway.TryGetServer() is null)
        {
            DownfallMainFile.Logger.Warn(
                "[Audio] deferred FMOD: FmodServer singleton missing; pending banks/GUID files kept for a later flush.");
            return;
        }

        List<string> banks;
        List<string> guidFiles;
        lock (QueueGate)
        {
            banks = [.. PendingBanks];
            guidFiles = [.. PendingGuidFiles];
            PendingBanks.Clear();
            PendingGuidFiles.Clear();
        }

        if (banks.Count == 0 && guidFiles.Count == 0)
            return;

        var failedBanks = new List<string>();
        var failedGuidFiles = new List<string>();

        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var path in banks)
            if (!TryLoadBank(path))
                failedBanks.Add(path);
        // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
        foreach (var path in guidFiles)
            if (!TryLoadGuidMappings(path))
                failedGuidFiles.Add(path);

        if (failedBanks.Count < banks.Count || failedGuidFiles.Count < guidFiles.Count)
            FmodStudioGateway.TryCall(WaitForAllLoads);

        if (failedBanks.Count > 0 || failedGuidFiles.Count > 0)
            lock (QueueGate)
            {
                PendingBanks.UnionWith(failedBanks);
                PendingGuidFiles.UnionWith(failedGuidFiles);
            }

        DownfallMainFile.Logger.Info(
            $"[Audio] deferred FMOD flush complete " +
            $"(banks={banks.Count - failedBanks.Count}/{banks.Count}, " +
            $"guid files={guidFiles.Count - failedGuidFiles.Count}/{guidFiles.Count}).");

        if (failedBanks.Count > 0 || failedGuidFiles.Count > 0)
            DownfallMainFile.Logger.Warn(
                $"[Audio] deferred FMOD flush retained {failedBanks.Count} bank(s) and " +
                $"{failedGuidFiles.Count} GUID file(s) for retry.");
    }

    private static bool TryLoadBank(string resourcePath, int mode = 0)
    {
        if (string.IsNullOrWhiteSpace(resourcePath) || !FileAccess.FileExists(resourcePath))
        {
            DownfallMainFile.Logger.Warn($"[Audio] load_bank: missing or empty path: {resourcePath}");
            return false;
        }

        if (!FmodStudioGateway.TryCall(out var result, LoadBank, resourcePath, mode))
        {
            DownfallMainFile.Logger.Warn($"[Audio] load_bank call failed: {resourcePath}");
            return false;
        }

        switch (result.VariantType)
        {
            case Variant.Type.Bool when result.AsBool():
                return true;

            case Variant.Type.Object:
            {
                var bank = result.AsGodotObject();
                if (bank is null || !GodotObject.IsInstanceValid(bank))
                    break;

                lock (BankPinsGate)
                    BankPins[resourcePath] = bank;
                return true;
            }
        }

        DownfallMainFile.Logger.Warn($"[Audio] load_bank did not succeed ({result.VariantType}): {resourcePath}");
        return false;
    }

    private static bool TryLoadGuidMappings(string guidMapResourcePath)
    {
        if (FmodStudioGuidPathTable.TryLoadFromResourceFile(guidMapResourcePath, out var parsed))
            return TryInjectGuidsNatively(guidMapResourcePath) || parsed > 0;

        DownfallMainFile.Logger.Warn(
            $"[Audio] guid map failed (missing, unreadable, or no event:/ mappings): {guidMapResourcePath}");
        return false;
    }

    private static bool TryInjectGuidsNatively(string resourcePath)
    {
        var server = FmodStudioGateway.TryGetServer();
        if (server is null)
            return false;

        foreach (var method in GuidInjectCandidates)
        {
            if (!server.HasMethod(method))
                continue;

            try
            {
                var result = server.Call(method, resourcePath);
                if (result.VariantType == Variant.Type.Bool && !result.AsBool())
                    continue;
                return true;
            }
            catch (Exception ex)
            {
                DownfallMainFile.Logger.Error($"[Audio] guid inject {method}: {ex.Message}");
            }
        }

        return false;
    }

    private static bool TryNormalizeGuid(string raw, out string bracedLowercase)
    {
        bracedLowercase = string.Empty;
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        var trimmed = raw.Trim();
        if (trimmed is ['{', _, ..] && trimmed[^1] == '}')
            trimmed = trimmed[1..^1].Trim();

        if (!Guid.TryParse(trimmed, out var guid))
            return false;

        bracedLowercase = guid.ToString("B");
        return true;
    }
}