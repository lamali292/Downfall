using Godot;
using MegaCrit.Sts2.Core.Platform.Steam;
using Steamworks;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// Persona name and avatar for a given Steam account, straight from the
/// Steamworks SDK the game already links (<see cref="UserIdentity"/> uses
/// the same <see cref="SteamInitializer"/> gate) - free, local, no Web API
/// key, no scraping steamcommunity.com. Takes an explicit
/// <see cref="CSteamID"/> rather than always assuming "the local player" -
/// the voting UI uses this to show the account backing the actual
/// authenticated <see cref="VotingAuth"/> session (via
/// <see cref="VotingApi.GetMySteamId"/>), which usually but not necessarily
/// matches whichever Steam client is running the game locally.
/// </summary>
public static class SteamAvatar
{
    private static bool IsAvailable => SteamInitializer.Initialized;

    private static readonly Dictionary<ulong, ImageTexture> AvatarCache = new();
    private static readonly Dictionary<ulong, TaskCompletionSource<ImageTexture?>> PendingAvatars = new();
    private static Callback<AvatarImageLoaded_t>? _avatarLoadedCallback;

    private static readonly Dictionary<ulong, TaskCompletionSource<string?>> PendingNames = new();
    private static Callback<PersonaStateChange_t>? _personaChangedCallback;

    public static string? LocalPersonaName => IsAvailable ? SteamFriends.GetPersonaName() : null;

    /// <summary>
    /// The persona name for <paramref name="steamId"/>, fetching it from
    /// Steam first if not already cached (only relevant for accounts other
    /// than the local player, whose own name is always immediately
    /// available).
    /// </summary>
    public static Task<string?> GetPersonaNameAsync(CSteamID steamId)
    {
        if (!IsAvailable)
            return Task.FromResult<string?>(null);

        if (steamId == SteamUser.GetSteamID())
            return Task.FromResult<string?>(SteamFriends.GetPersonaName());

        // Returns false when the info is already cached - no need to wait.
        if (!SteamFriends.RequestUserInformation(steamId, bRequireNameOnly: true))
            return Task.FromResult<string?>(SteamFriends.GetFriendPersonaName(steamId));

        if (!PendingNames.TryGetValue(steamId.m_SteamID, out var tcs))
            PendingNames[steamId.m_SteamID] = tcs = new TaskCompletionSource<string?>();

        _personaChangedCallback ??= Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);
        return tcs.Task;
    }

    private static void OnPersonaStateChange(PersonaStateChange_t result)
    {
        if (!PendingNames.Remove(result.m_ulSteamID, out var tcs))
            return;

        tcs.TrySetResult(SteamFriends.GetFriendPersonaName(new CSteamID(result.m_ulSteamID)));
    }

    /// <summary>
    /// The avatar for <paramref name="steamId"/> as a ready-to-use texture,
    /// or null if Steam isn't running or the account has no avatar set.
    /// Steam sometimes hasn't cached the image yet on first call
    /// (<c>GetMediumFriendAvatar</c> returns -1) - this waits for the
    /// one-shot <see cref="AvatarImageLoaded_t"/> callback and retries in
    /// that case, so callers never see the "loading" state themselves.
    /// </summary>
    public static Task<ImageTexture?> GetAvatarAsync(CSteamID steamId)
    {
        if (AvatarCache.TryGetValue(steamId.m_SteamID, out var cached))
            return Task.FromResult<ImageTexture?>(cached);

        if (!IsAvailable)
            return Task.FromResult<ImageTexture?>(null);

        var handle = SteamFriends.GetMediumFriendAvatar(steamId);
        if (handle != -1)
            return Task.FromResult(BuildTexture(steamId, handle));

        // Still loading - wait for Steam to fetch it, then retry once.
        if (!PendingAvatars.TryGetValue(steamId.m_SteamID, out var tcs))
            PendingAvatars[steamId.m_SteamID] = tcs = new TaskCompletionSource<ImageTexture?>();

        _avatarLoadedCallback ??= Callback<AvatarImageLoaded_t>.Create(OnAvatarLoaded);
        return tcs.Task;
    }

    private static void OnAvatarLoaded(AvatarImageLoaded_t result)
    {
        // This callback fires for any avatar Steam finishes loading (e.g. a
        // friend's, elsewhere in the game), not just the one we're waiting on.
        if (!PendingAvatars.Remove(result.m_steamID.m_SteamID, out var tcs))
            return;

        tcs.TrySetResult(BuildTexture(result.m_steamID, result.m_iImage));
    }

    private static ImageTexture? BuildTexture(CSteamID steamId, int handle)
    {
        if (handle <= 0)
            return null;

        if (!SteamUtils.GetImageSize(handle, out var width, out var height) || width == 0 || height == 0)
            return null;

        var buffer = new byte[width * height * 4];
        if (!SteamUtils.GetImageRGBA(handle, buffer, buffer.Length))
            return null;

        var image = Image.CreateFromData((int)width, (int)height, false, Image.Format.Rgba8, buffer);
        var texture = ImageTexture.CreateFromImage(image);
        AvatarCache[steamId.m_SteamID] = texture;
        return texture;
    }
}
