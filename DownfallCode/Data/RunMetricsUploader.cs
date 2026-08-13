using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Saves;

namespace Downfall.DownfallCode.Data;

using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Text.Json;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.Metrics;



/// <summary>
/// All mod-specific policy for a metrics uploader. The library owns the mechanism;
/// this owns the decisions.
/// </summary>
public sealed class MetricsUploaderConfig
{
    /// <summary>Used in log messages and passed to <see cref="WrapPayload"/>.</summary>
    public required string ModName { get; init; }

    /// <summary>Full REST endpoint the payload is POSTed to.</summary>
    public required string EndpointUrl { get; init; }

    /// <summary>
    /// Optional API key. When set and <see cref="ConfigureRequest"/> is null, the default
    /// Supabase-style headers (apikey + Bearer + Prefer: return=minimal) are applied.
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>Returns the mod version string embedded in the wrapped payload.</summary>
    public Func<string> ModVersionProvider { get; init; } = () => "unknown";

    /// <summary>
    /// Optional predicate deciding whether the local player's character belongs to this mod.
    /// If null, character is not gated. Use <see cref="MetricsPredicates"/> to build one.
    /// </summary>
    public Func<CharacterModel, bool>? IsOwnCharacter { get; init; }

    /// <summary>
    /// Assemblies whose content is considered "known". A run containing any card/relic/
    /// potion/character from an assembly outside this set is rejected when
    /// <see cref="RejectForeignContent"/> is true. Typically your mod's assembly plus the
    /// base-game assembly (typeof(CharacterModel).Assembly).
    /// </summary>
    public IReadOnlySet<Assembly>? AllowedAssemblies { get; init; }

    public int MinFloors { get; init; } = 5;
    public bool RequireStandardGameMode { get; init; } = true;
    public bool SkipAbandonedRuns { get; init; } = true;
    public bool RejectForeignContent { get; init; } = true;

    /// <summary>Force English entries while the payload is built (recommended for shared analytics).</summary>
    public bool OverrideLanguageToEnglish { get; init; } = true;

    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(15);
    public Logger? Logger { get; init; }

    /// <summary>
    /// Optional custom envelope: (dataJson, modVersion, modName) => body.
    /// Defaults to {"mod_version": &lt;version&gt;, "data": &lt;dataJson&gt;}.
    /// </summary>
    public Func<string, string, string, string>? WrapPayload { get; init; }

    /// <summary>
    /// Optional custom header/auth setup. When null, Supabase-style headers are applied
    /// if <see cref="ApiKey"/> is set. Use this for any non-Supabase backend.
    /// </summary>
    public Action<HttpRequestMessage>? ConfigureRequest { get; init; }
}

/// <summary>Helpers for building the <see cref="MetricsUploaderConfig.IsOwnCharacter"/> predicate.</summary>
public static class MetricsPredicates
{
    /// <summary>Own characters are any whose model type is <typeparamref name="T"/>.</summary>
    public static Func<CharacterModel, bool> CharacterOfType<T>() where T : CharacterModel
        => c => c is T;

    /// <summary>Own characters are any defined in the given assembly.</summary>
    public static Func<CharacterModel, bool> CharacterFromAssembly(Assembly assembly)
        => c => c.GetType().Assembly == assembly;
}

/// <summary>
/// Uploads run metrics for a single mod. Generic over the payload type so each mod keeps
/// its own schema and (source-generated) serializer.
/// </summary>
/// <typeparam name="TPayload">The mod's metrics DTO that gets serialized and sent.</typeparam>
public sealed class RunMetricsUploader<TPayload>
{
    private readonly MetricsUploaderConfig _config;
    private readonly Func<SerializableRun, bool, ulong, TPayload> _buildPayload;
    private readonly Func<TPayload, string> _serialize;

    /// <param name="config"></param>
    /// <param name="buildPayload">
    /// Walks the run and produces the DTO. Called inside the (optional) English-language
    /// override, so it must not manage that itself.
    /// </param>
    /// <param name="serialize">
    /// Serializes the DTO to JSON. Pass your source-generated context here to stay
    /// AOT/trim-safe, e.g. m =&gt; JsonSerializer.Serialize(m, MyContext.Default.RunMetrics).
    /// </param>
    public RunMetricsUploader(
        MetricsUploaderConfig config,
        Func<SerializableRun, bool, ulong, TPayload> buildPayload,
        Func<TPayload, string> serialize)
    {
        _config = config;
        _buildPayload = buildPayload;
        _serialize = serialize;
    }

    /// <summary>Gate, build, and fire-and-forget the upload. Safe to call from your hook.</summary>
    public void Upload(SerializableRun run, bool isVictory, ulong localPlayerId)
    {
        if (!ShouldUpload(run, localPlayerId)) return;

        TPayload payload;
        if (_config.OverrideLanguageToEnglish)
        {
            LocManager.Instance.StartOverridingLanguageAsEnglish();
            try { payload = _buildPayload(run, isVictory, localPlayerId); }
            finally { LocManager.Instance.StopOverridingLanguageAsEnglish(); }
        }
        else
        {
            payload = _buildPayload(run, isVictory, localPlayerId);
        }

        var json = _serialize(payload);
        _ = SendToServer(json);
    }

    private bool ShouldUpload(SerializableRun run, ulong localPlayerId)
    {
        var log = _config.Logger;
        var mod = _config.ModName;

        if (!MetricUtilities.ShouldUploadMetrics()) return false;

        if (_config.SkipAbandonedRuns && RunManager.Instance.IsAbandoned)
        {
            log?.Info($"[{mod}] Skipping metrics upload, run was abandoned.");
            return false;
        }

        if (_config.RequireStandardGameMode && run.GameMode != GameMode.Standard)
        {
            log?.Info($"[{mod}] Skipping metrics upload, custom mode detected.");
            return false;
        }

        if (run.MapPointHistory.SelectMany(logs => logs).Count() < _config.MinFloors)
        {
            log?.Info($"[{mod}] Skipping metrics upload, not enough progress.");
            return false;
        }

        var localPlayer = run.Players.FirstOrDefault(p => (long)p.NetId == (long)localPlayerId);
        if (localPlayer?.CharacterId is not { } charId)
        {
            log?.Info($"[{mod}] Skipping metrics upload, no local player found.");
            return false;
        }

        if (_config.IsOwnCharacter is { } isOwn)
        {
            var characterModel = ModelDb.GetByIdOrNull<CharacterModel>(charId);
            if (characterModel == null || !isOwn(characterModel))
            {
                log?.Info($"[{mod}] Skipping metrics upload, active character isn't owned by this mod.");
                return false;
            }
        }

        if (_config.RejectForeignContent && HasForeignContent(run))
        {
            log?.Info($"[{mod}] Skipping metrics upload, foreign mod content detected.");
            return false;
        }

        return true;
    }

    /// <summary>
    /// True if any player's deck/relics/potions/character comes from an assembly outside
    /// <see cref="MetricsUploaderConfig.AllowedAssemblies"/>. Note: only inspects player-facing
    /// content, not map encounters/events.
    /// </summary>
    private bool HasForeignContent(SerializableRun run)
    {
        if (run.Acts.Any(c => !IsAllowed<ActModel>(c.Id))) return true;
        foreach (var p in run.Players)
        {
            if (!IsAllowed<CharacterModel>(p.CharacterId)) return true;
            if (p.Deck.Any(c => !IsAllowed<CardModel>(c.Id))) return true;
            if (p.Relics.Any(r => !IsAllowed<RelicModel>(r.Id))) return true;
            if (p.Potions.Any(pot => !IsAllowed<PotionModel>(pot.Id))) return true;
        }
        return false;
    }

    private bool IsAllowed<T>(ModelId? id) where T : AbstractModel
    {
        if (_config.AllowedAssemblies == null) return true;
        if (id is null || id == ModelId.none) return true;
        var model = ModelDb.GetByIdOrNull<T>(id);
        return model != null && _config.AllowedAssemblies.Contains(model.GetType().Assembly);
    }

    private async Task SendToServer(string dataJson)
    {
        var log = _config.Logger;
        var mod = _config.ModName;
        try
        {
            log?.Info($"[{mod}] Start uploading metrics!");

            var version = _config.ModVersionProvider();
            var body = _config.WrapPayload is { } wrap
                ? wrap(dataJson, version, mod)
                : DefaultWrap(dataJson, version);

            var bytes = Encoding.UTF8.GetBytes(body);

            using var client = new HttpClient();
            client.Timeout = _config.Timeout;
            using var content = new ByteArrayContent(bytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            using var request = new HttpRequestMessage(HttpMethod.Post, _config.EndpointUrl);
            request.Content = content;

            if (_config.ConfigureRequest is { } configure)
                configure(request);
            else
                ApplyDefaultHeaders(request);

            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            log?.Info($"[{mod}] POST returned: {(int)response.StatusCode} {response.StatusCode}");

            if (response.IsSuccessStatusCode)
                log?.Info($"[{mod}] Upload successful! {responseBody}");
            else
                log?.Warn($"[{mod}] Upload failed: {response.StatusCode} {responseBody}");
        }
        catch (Exception ex)
        {
            log?.Error($"[{mod}] Metrics upload exception: {ex}");
        }
        finally
        {
            log?.Info($"[{mod}] End uploading metrics!");
        }
    }

    private static string DefaultWrap(string dataJson, string version)
        => $"{{\"mod_version\":{JsonSerializer.Serialize(version)},\"data\":{dataJson}}}";

    private void ApplyDefaultHeaders(HttpRequestMessage request)
    {
        if (_config.ApiKey is not { } key) return;
        request.Headers.Add("apikey", key);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        request.Headers.Add("Prefer", "return=minimal");
    }
}