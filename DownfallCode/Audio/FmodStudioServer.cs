using System.Security.Cryptography;
using System.Text;
using Godot;
using FileAccess = Godot.FileAccess;

namespace Downfall.DownfallCode.Audio;

public static class FmodStudioServer
{
    private static readonly Lock LoadedBankPinsGate = new();

    private static readonly Dictionary<string, GodotObject> LoadedBankPins = [];

    private static readonly StringName BankGetGodotResourcePath = new("get_godot_res_path");
    private static readonly StringName BankGetEventDescriptionCount = new("get_event_description_count");
    private static readonly StringName BankGetDescriptionList = new("get_description_list");
    private static readonly StringName EventDescriptionGetPath = new("get_path");

    private static readonly StringName[] GuidMappingInjectCandidates =
    [
        new("register_guid_path_mappings_from_file"),
        new("inject_guid_mappings_from_file"),
        new("register_strings_from_guid_file"),
        new("load_guid_mapping_file"),
    ];
    
    public static GodotObject? TryGet()
    {
        return FmodStudioGateway.TryGetServer();
    }
    
    public static bool TryLoadBank(string resourcePath, FmodStudioLoadBankMode mode = FmodStudioLoadBankMode.Normal)
    {
        if (string.IsNullOrWhiteSpace(resourcePath))
        {
            DownfallMainFile.Logger.Warn("[Audio] FMOD load_bank: empty path.");
            return false;
        }

        if (!FileAccess.FileExists(resourcePath))
        {
            DownfallMainFile.Logger.Warn(
                $"[Audio] FMOD load_bank: file not found: {resourcePath}; {DescribeResourceForDiagnostics(resourcePath)}");
            return false;
        }

        if (!FmodStudioGateway.TryCall(out var result, FmodStudioMethodNames.LoadBank, resourcePath, (int)mode))
        {
            DownfallMainFile.Logger.Warn(
                $"[Audio] FMOD load_bank call failed: {resourcePath}; {DescribeResourceForDiagnostics(resourcePath)}");
            return false;
        }

        switch (result.VariantType)
        {
            case Variant.Type.Bool:
                if (result.AsBool())
                    return true;

                DownfallMainFile.Logger.Warn(
                    $"[Audio] FMOD load_bank returned false: {resourcePath}; {DescribeResourceForDiagnostics(resourcePath)}");
                return false;
            case Variant.Type.Nil:
                DownfallMainFile.Logger.Warn(
                    $"[Audio] FMOD load_bank returned nil: {resourcePath}; {DescribeResourceForDiagnostics(resourcePath)}");
                return false;
            case Variant.Type.Object:
            {
                var bank = result.AsGodotObject();
                if (bank is null || !GodotObject.IsInstanceValid(bank))
                {
                    DownfallMainFile.Logger.Warn(
                        $"[Audio] FMOD load_bank returned invalid {result.VariantType}: {resourcePath}; {DescribeResourceForDiagnostics(resourcePath)}");
                    return false;
                }

                lock (LoadedBankPinsGate)
                {
                    LoadedBankPins[resourcePath] = bank;
                }

                return true;
            }
            default:
                DownfallMainFile.Logger.Warn(
                    $"[Audio] FMOD load_bank returned unsupported {result.VariantType}: {resourcePath}; {DescribeResourceForDiagnostics(resourcePath)}");
                return false;
        }
    }
    
    public static void LogBankResourceDiagnostics(string resourcePath)
    {
        DownfallMainFile.Logger.Info(
            $"[Audio] FMOD bank resource diagnostics: {resourcePath}; {DescribeResourceForDiagnostics(resourcePath)}");
    }
    
    public static bool TryUnloadBank(string resourcePath)
    {
        if (string.IsNullOrWhiteSpace(resourcePath))
            return false;

        bool hadPin;
        lock (LoadedBankPinsGate)
        {
            hadPin = LoadedBankPins.Remove(resourcePath);
        }

        return hadPin || FmodStudioGateway.TryCall(FmodStudioMethodNames.UnloadBank, resourcePath);
    }
    
    public static void TryWaitForAllLoads()
    {
        FmodStudioGateway.TryCall(FmodStudioMethodNames.WaitForAllLoads);
    }
    
    public static bool? TryBanksStillLoading()
    {
        if (!FmodStudioGateway.TryCall(out var v, FmodStudioMethodNames.BanksStillLoading))
            return null;

        return v.VariantType == Variant.Type.Bool ? v.AsBool() : null;
    }

   
    public static bool TryLoadStudioGuidMappings(string guidMapResourcePath)
    {
        if (string.IsNullOrWhiteSpace(guidMapResourcePath))
        {
            DownfallMainFile.Logger.Warn("[Audio] FMOD guid map: empty path.");
            return false;
        }

        if (!FileAccess.FileExists(guidMapResourcePath))
        {
            DownfallMainFile.Logger.Warn($"[Audio] FMOD guid map file not found: {guidMapResourcePath}");
            return false;
        }

        if (!TryApplyStudioGuidMappingsCore(guidMapResourcePath))
        {
            DownfallMainFile.Logger.Warn(
                $"[Audio] FMOD guid map failed (unreadable or no usable event:/ mappings): {guidMapResourcePath}");
            return false;
        }

        var n = FmodStudioGuidPathTable.EventMappingCount;
        DownfallMainFile.Logger.Info($"[Audio] FMOD guid map OK: {guidMapResourcePath} ({n} event path(s))");
        return true;
    }
    
    public static bool TryInjectStudioGuidMappings(string resourcePath)
    {
        if (TryApplyStudioGuidMappingsCore(resourcePath)) return true;
        DownfallMainFile.Logger.Warn($"[Audio] FMOD guid map could not be applied: {resourcePath}");
        return false;
    }

    private static bool TryApplyStudioGuidMappingsCore(string resourcePath)
    {
        if (!FmodStudioGuidPathTable.TryLoadFromResourceFile(resourcePath, out var parsedEventMappings))
            return false;

        var injected = TryCallNativeGuidInject(resourcePath);
        WarnIfMappedEventGuidsUnresolved();
        return injected || parsedEventMappings > 0;
    }

    private static void WarnIfMappedEventGuidsUnresolved()
    {
        foreach (var (path, guid) in FmodStudioGuidPathTable.SnapshotEventMappings())
        {
            if (TryCheckEventGuid(guid) != false)
                continue;

            DownfallMainFile.Logger.Warn(
                "[Audio] guids.txt: GUID not found in loaded FMOD Studio data — " +
                $"event '{path}', GUID '{guid}'. Load matching banks before injection and regenerate GUIDs.txt from the same build.");
        }
    }
    
    public static bool? TryCheckEventPath(string eventPath)
    {
        if (string.IsNullOrWhiteSpace(eventPath))
            return false;

        if (FmodStudioGuidPathTable.TryGetStudioGuidForEventPath(eventPath, out _))
            return true;

        if (!FmodStudioGateway.TryCall(out var v, FmodStudioMethodNames.CheckEventPath, eventPath))
            return null;

        return v.VariantType == Variant.Type.Bool ? v.AsBool() : null;
    }

    
    public static bool? TryCheckBusPath(string busPath)
    {
        if (string.IsNullOrWhiteSpace(busPath))
            return false;

        if (!FmodStudioGateway.TryCall(out var v, FmodStudioMethodNames.CheckBusPath, busPath))
            return null;

        return v.VariantType == Variant.Type.Bool ? v.AsBool() : null;
    }

   
    public static GodotObject? TryGetEventDescriptionFromGuid(string eventGuid)
    {
        if (string.IsNullOrWhiteSpace(eventGuid))
            return null;

        if (!FmodStudioGuidInterop.TryNormalizeForAddon(eventGuid, out var normalized))
            return null;

        if (!FmodStudioGateway.TryCall(out var v, FmodStudioMethodNames.GetEventFromGuid, normalized))
            return null;

        if (v.VariantType != Variant.Type.Object)
            return null;

        var description = v.AsGodotObject();
        return description is not null && GodotObject.IsInstanceValid(description) ? description : null;
    }

   
    public static bool? TryCheckEventGuid(string eventGuid)
    {
        if (!FmodStudioGuidInterop.TryNormalizeForAddon(eventGuid, out var normalized))
            return null;

        if (!FmodStudioGateway.TryCall(out var v, FmodStudioMethodNames.CheckEventGuid, normalized))
            return null;

        return v.VariantType == Variant.Type.Bool ? v.AsBool() : null;
    }

   
    public static Godot.Collections.Array TryGetAllBuses()
    {
        if (!FmodStudioGateway.TryCall(out var v, FmodStudioMethodNames.GetAllBuses))
            return [];

        return v.VariantType == Variant.Type.Array ? v.AsGodotArray() : [];
    }

   
    public static int TryGetLoadedBankCount()
    {
        if (!FmodStudioGateway.TryCall(out var v, FmodStudioMethodNames.GetAllBanks))
            return -1;

        return v.VariantType == Variant.Type.Array ? v.AsGodotArray().Count : -1;
    }
    
    public static int TryGetLoadedEventDescriptionCount()
    {
        if (!FmodStudioGateway.TryCall(out var v, FmodStudioMethodNames.GetAllEventDescriptions))
            return -1;

        return v.VariantType == Variant.Type.Array ? v.AsGodotArray().Count : -1;
    }

   
    public static long TryGetLoadedBankEventDescriptionCount(string bankResourcePath)
    {
        if (string.IsNullOrWhiteSpace(bankResourcePath))
            return -1;

        if (!FmodStudioGateway.TryCall(out var banksVar, FmodStudioMethodNames.GetAllBanks))
            return -1;

        if (banksVar.VariantType != Variant.Type.Array)
            return -1;

        foreach (var item in banksVar.AsGodotArray())
        {
            if (item.VariantType != Variant.Type.Object)
                continue;

            var bank = item.AsGodotObject();
            if (bank is null || !GodotObject.IsInstanceValid(bank) ||
                !bank.HasMethod(BankGetGodotResourcePath))
                continue;

            string path;
            try
            {
                path = bank.Call(BankGetGodotResourcePath).AsString();
            }
            catch (Exception ex)
            {
                DownfallMainFile.Logger.Error(
                    $"[Audio] FMOD bank resource-path inspection: {ex}");
                continue;
            }

            if (!string.Equals(path, bankResourcePath, StringComparison.Ordinal))
                continue;

            if (!bank.HasMethod(BankGetEventDescriptionCount))
                return -1;

            try
            {
                return bank.Call(BankGetEventDescriptionCount).AsInt64();
            }
            catch (Exception ex)
            {
                DownfallMainFile.Logger.Error(
                    $"[Audio] FMOD bank event-count inspection: {ex}");
                return -1;
            }
        }

        return -1;
    }
    
    public static void TryLogLoadedStudioBankEvents(string bankResourcePath)
    {
        if (string.IsNullOrWhiteSpace(bankResourcePath))
            return;

        var paths = TryCollectLoadedBankEventPaths(bankResourcePath);
        if (paths is null)
        {
            DownfallMainFile.Logger.Warn($"[Audio] FMOD bank not loaded or unreadable: {bankResourcePath}");
            return;
        }

        if (paths.Count == 0)
        {
            DownfallMainFile.Logger.Warn(
                "[Audio] FMOD bank has no events — rebuild banks from FMOD Studio or verify the exported .bank.");
            return;
        }

        const int maxListed = 40;
        var sb = new StringBuilder(256);
        var n = Math.Min(paths.Count, maxListed);
        for (var i = 0; i < n; i++)
        {
            if (i > 0)
                sb.Append(", ");

            sb.Append(paths[i]);
        }

        if (paths.Count > maxListed)
            sb.Append(" … (+").Append(paths.Count - maxListed).Append(" more)");

        DownfallMainFile.Logger.Info(
            $"[Audio] FMOD bank {bankResourcePath} ({paths.Count} event{(paths.Count == 1 ? "" : "s")}): {sb}");
    }

    private static List<string>? TryCollectLoadedBankEventPaths(string bankResourcePath)
    {
        if (!FmodStudioGateway.TryCall(out var banksVar, FmodStudioMethodNames.GetAllBanks) ||
            banksVar.VariantType != Variant.Type.Array)
            return null;

        foreach (var item in banksVar.AsGodotArray())
        {
            if (item.VariantType != Variant.Type.Object)
                continue;

            var bank = item.AsGodotObject();
            if (bank is null || !GodotObject.IsInstanceValid(bank) ||
                !bank.HasMethod(BankGetGodotResourcePath))
                continue;

            string resPath;
            try
            {
                resPath = bank.Call(BankGetGodotResourcePath).AsString();
            }
            catch (Exception ex)
            {
                DownfallMainFile.Logger.Error(
                    $"[Audio] FMOD bank resource-path enumeration: {ex}");
                continue;
            }

            if (!string.Equals(resPath, bankResourcePath, StringComparison.Ordinal))
                continue;

            if (!bank.HasMethod(BankGetDescriptionList))
                return null;

            var paths = new List<string>();
            try
            {
                var listVar = bank.Call(BankGetDescriptionList);
                if (listVar.VariantType != Variant.Type.Array)
                    return null;

                foreach (var descriptionValue in listVar.AsGodotArray())
                {
                    if (descriptionValue.VariantType != Variant.Type.Object)
                        return null;

                    var description = descriptionValue.AsGodotObject();
                    if (description is null || !GodotObject.IsInstanceValid(description) ||
                        !description.HasMethod(EventDescriptionGetPath))
                        return null;

                    paths.Add(description.Call(EventDescriptionGetPath).AsString());
                }
            }
            catch (Exception ex)
            {
                DownfallMainFile.Logger.Error(
                    $"[Audio] FMOD bank event-path enumeration: {ex}");
                return null;
            }

            return paths;
        }

        return null;
    }

    private static bool TryCallNativeGuidInject(string resourcePath)
    {
        var server = FmodStudioGateway.TryGetServer();
        if (server is null)
            return false;

        foreach (var method in GuidMappingInjectCandidates)
        {
            if (!server.HasMethod(method))
                continue;

            try
            {
                var r = server.Call(method, resourcePath);
                if (r.VariantType == Variant.Type.Bool && !r.AsBool())
                    continue;

                return true;
            }
            catch (Exception ex)
            {
                DownfallMainFile.Logger.Error($"[Audio] FMOD guid inject {method}: {ex}");
            }
        }

        return false;
    }

    private static string DescribeResourceForDiagnostics(string resourcePath)
    {
        var parts = new List<string>
        {
            $"fileExists={FileAccess.FileExists(resourcePath)}",
            $"resourceExists={ResourceLoader.Exists(resourcePath)}",
        };

        try
        {
            var bytes = FileAccess.GetFileAsBytes(resourcePath);
            parts.Add($"bytes={bytes.Length}");
            if (bytes.Length > 0)
            {
                parts.Add($"sha256={Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant()}");
                parts.Add($"head={DescribeHead(bytes)}");
            }
        }
        catch (Exception ex)
        {
            parts.Add($"readError={ex}");
        }

        try
        {
            var resource = ResourceLoader.Load<Resource>(resourcePath);
            parts.Add(resource is null
                ? "resourceType=<null>"
                : $"resourceType={resource.GetClass()}; resourcePath={resource.ResourcePath}");
        }
        catch (Exception ex)
        {
            parts.Add($"resourceLoadError={ex}");
        }

        try
        {
            var globalized = ProjectSettings.GlobalizePath(resourcePath);
            if (!string.IsNullOrWhiteSpace(globalized) &&
                !string.Equals(globalized, resourcePath, StringComparison.Ordinal))
                parts.Add($"globalized={globalized}");
        }
        catch (Exception ex)
        {
            parts.Add($"globalizeError={ex}");
        }

        parts.Add("nativeResult=unavailable-from-managed-wrapper");
        return string.Join("; ", parts);
    }

    private static string DescribeHead(byte[] bytes)
    {
        var n = Math.Min(bytes.Length, 16);
        var hex = Convert.ToHexString(bytes, 0, n).ToLowerInvariant();
        var ascii = new char[n];
        for (var i = 0; i < n; i++)
            ascii[i] = bytes[i] is >= 32 and <= 126 ? (char)bytes[i] : '.';

        return $"{hex}/{new string(ascii)}";
    }
}