using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;

namespace Downfall.DownfallCode.Audio;

[HarmonyPatch(typeof(OneTimeInitialization), nameof(OneTimeInitialization.ExecuteDeferred))]
internal static class DeferredInitializationFmodFlushPatch
{
    [HarmonyPostfix]
    public static void Postfix()
    { 
        try
        {
            FmodStudioDeferredBankRegistration.OnDeferredInitializationCompleted();
        }
        catch (Exception ex)
        {
            DownfallMainFile.Logger.Warn($"[Audio] deferred FMOD flush hook failed: {ex.Message}");
        }
        foreach (var busVar in FmodStudioServer.TryGetAllBuses())
        {
            var bus = busVar.AsGodotObject();
            if (bus is null || !GodotObject.IsInstanceValid(bus)) continue;
            // FMOD addon bus objects expose a path getter — try "get_path"
            if (bus.HasMethod("get_path"))
                DownfallMainFile.Logger.Info($"[Downfall] bus: {bus.Call("get_path").AsString()}");
            else
            {
                // dump available methods to find the right getter
                foreach (var m in bus.GetMethodList())
                    DownfallMainFile.Logger.Info($"[Downfall]   method: {m["name"].AsString()}");
            }
        }
    }
}

 public static class FmodStudioDeferredBankRegistration
    {
        private static readonly Lock Gate = new();
        private static readonly Lock FlushGate = new();
        private static readonly HashSet<string> PendingBanks = new(StringComparer.Ordinal);
        private static readonly HashSet<string> PendingGuidFiles = new(StringComparer.Ordinal);
        private static bool _deferredInitCompleted;

        public static void RegisterBank(string resourcePath)
        {
            if (string.IsNullOrWhiteSpace(resourcePath))
                return;

            bool flushNow;
            lock (Gate)
            {
                PendingBanks.Add(resourcePath.Trim());
                flushNow = _deferredInitCompleted;
            }

            if (flushNow)
                FlushPending();
        }

        public static void RegisterStudioGuidMappings(string guidMapResourcePath)
        {
            if (string.IsNullOrWhiteSpace(guidMapResourcePath))
                return;

            bool flushNow;
            lock (Gate)
            {
                PendingGuidFiles.Add(guidMapResourcePath.Trim());
                flushNow = _deferredInitCompleted;
            }

            if (flushNow)
                FlushPending();
        }

        public static void OnDeferredInitializationCompleted()
        {
            lock (Gate)
            {
                _deferredInitCompleted = true;
            }

            FlushPending();
        }


        private static void FlushPending()
        {
            lock (FlushGate)
            {
                FlushPendingCore();
            }
        }

        private static void FlushPendingCore()
        {
            if (FmodStudioServer.TryGet() is null)
            {
                DownfallMainFile.Logger.Warn(
                    "[Audio] deferred FMOD: FmodServer singleton missing; pending banks/GUID files kept for a later flush."
                );
                return;
            }

            List<string> banks;
            List<string> guids;

            lock (Gate)
            {
                banks = [.. PendingBanks];
                guids = [.. PendingGuidFiles];
                PendingBanks.Clear();
                PendingGuidFiles.Clear();
            }

            if (banks.Count == 0 && guids.Count == 0)
                return;

            var failedBanks = new List<string>();
            var failedGuids = new List<string>();

            // Preserve the concrete collection enumerator and straightforward failure accumulation.
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var path in banks)
                if (!FmodStudioServer.TryLoadBank(path))
                    failedBanks.Add(path);

            // Preserve the concrete collection enumerator and straightforward failure accumulation.
            // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
            foreach (var path in guids)
                if (!FmodStudioServer.TryLoadStudioGuidMappings(path))
                    failedGuids.Add(path);

            if (failedBanks.Count < banks.Count || failedGuids.Count < guids.Count)
                FmodStudioServer.TryWaitForAllLoads();

            if (failedBanks.Count > 0 || failedGuids.Count > 0)
                lock (Gate)
                {
                    PendingBanks.UnionWith(failedBanks);
                    PendingGuidFiles.UnionWith(failedGuids);
                }

            DownfallMainFile.Logger.Info(
                $"[Audio] deferred FMOD flush complete " +
                $"(banks={banks.Count - failedBanks.Count}/{banks.Count}, " +
                $"guid files={guids.Count - failedGuids.Count}/{guids.Count})."
            );

            if (failedBanks.Count > 0 || failedGuids.Count > 0)
                DownfallMainFile.Logger.Warn(
                    $"[Audio] deferred FMOD flush retained {failedBanks.Count} bank(s) and " +
                    $"{failedGuids.Count} GUID file(s) for retry."
                );
        }

       
    }