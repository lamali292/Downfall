using System.Runtime.CompilerServices;
using System.Text;
using Downfall.DownfallCode.Multiplayer;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Transport.Steam;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.TestSupport;

namespace Downfall.DownfallCode.Patches;

/// <summary>
/// Adds our disabled-character set to the non-gameplay mod list of the handshake we send (compared for warnings
/// only). Patched on <c>Serialize</c> rather than on the mod-list getters: those are small enough to be inlined into
/// <c>PeerVersionInfo.LocalDefault</c>, where a patch would silently never run.
/// </summary>
[HarmonyPatch(typeof(PeerVersionInfo), nameof(PeerVersionInfo.Serialize))]
internal static class AdvertiseDisabledCharactersPatch
{
    [HarmonyPrefix]
    private static void Prefix(ref PeerVersionInfo __instance)
    {
        __instance.otherMods ??= [];
        __instance.otherMods.RemoveAll(DisabledCharacterSync.IsMarker);
        __instance.otherMods.Add(DisabledCharacterSync.BuildMarker(DisabledCharacterSync.LocalDisabled()));
    }
}

/// <summary>
/// Remembers the Steam lobby of a join whose handshake just failed. The client leaves the lobby right after, so this
/// is the last moment the id is still available for rejoining after a restart.
/// </summary>
[HarmonyPatch(typeof(NetClientGameService), nameof(NetClientGameService.HandshakeFailed))]
internal static class CaptureFailedJoinLobbyPatch
{
    [HarmonyPrefix]
    private static void Prefix(NetClientGameService __instance)
    {
        DisabledCharacterSync.FailedJoinLobbyId = (__instance.NetClient as SteamClient)?.LobbyId?.m_SteamID;
    }
}

/// <summary>
/// When joining fails on a mod/hash mismatch and the host's disabled characters differ from ours, replace the generic
/// error with a popup that offers to adopt the host's settings and restart.
/// </summary>
[HarmonyPatch(typeof(NErrorPopup), nameof(NErrorPopup.Create), typeof(NetErrorInfo))]
internal static class CharacterMismatchPopupPatch
{
    internal sealed record PendingApply(List<string> Disabled, ulong? LobbyId);

    /// <summary>Popups created here, with the host's disabled set they should apply.</summary>
    internal static readonly ConditionalWeakTable<NErrorPopup, PendingApply> HostSettings = new();

    [HarmonyPrefix]
    private static bool Prefix(NetErrorInfo info, ref NErrorPopup? __result)
    {
        if (TestMode.IsOn || DisabledCharacterSync.JustSynced) return true;

        var extra = info.ConnectionExtraInfo;
        if (extra == null || extra.localIsHost) return true;

        var reason = info.GetReason();
        if (reason != NetError.VersionMismatch && reason != NetError.ModMismatch) return true;

        var hasMarker = DisabledCharacterSync.TryParse(extra.remoteInfo.otherMods, out var host);
        var local = DisabledCharacterSync.LocalDisabled();
        DownfallMainFile.Logger.Info(
            $"[CharacterSync] Join failed ({reason}); host advertises disabled set: {(hasMarker ? $"[{string.Join(",", host)}]" : "none (older build?)")}, ours: [{string.Join(",", local)}].");
        if (!hasMarker || host.SequenceEqual(local)) return true;

        // Below the character table, keep only the game's "More info: ... ModelDb hash ..." line: the popup has no room
        // for its long intro sentence. Fall back to the full text when the hashes are not what differs.
        var characters = new LocString("settings_ui", "DOWNFALL-CHARACTER_MISMATCH.body");
        characters.Add("table", BuildTable(host, local));
        var originalText = HashMismatchLine(extra) ?? NErrorPopup.LocStringFromNetError(info, out _).GetFormattedText();
        var body = new LocString("settings_ui", "DOWNFALL-CHARACTER_MISMATCH.combined");
        body.Add("original", originalText);
        body.Add("characters", characters.GetFormattedText());
        var popup = NErrorPopup.Create(new LocString("settings_ui", "DOWNFALL-CHARACTER_MISMATCH.title"), body,
            new LocString("settings_ui", "DOWNFALL-CHARACTER_MISMATCH.cancel"), false);
        if (popup == null) return true;

        HostSettings.Add(popup, new PendingApply(host, DisabledCharacterSync.FailedJoinLobbyId));
        __result = popup;
        return false;
    }

    /// <summary>The game's own "More info: ... ModelDb hash ..." line; the reason a join with different characters fails.</summary>
    private static string? HashMismatchLine(ConnectionFailureExtraInfo extra)
    {
        if (extra.remoteInfo.idDatabaseHash == extra.localInfo.idDatabaseHash) return null;

        var line = new LocString("main_menu_ui", "NETWORK_ERROR.VERSION_MISMATCH.description.modelDbMismatch");
        line.Add("remoteHash", extra.remoteInfo.idDatabaseHash.ToString());
        line.Add("localHash", extra.localInfo.idDatabaseHash.ToString());
        return line.GetFormattedText();
    }

    /// <summary>
    /// One row per character whose state differs between host and local, ticks for what is enabled. Matching
    /// characters are left out to keep the popup short.
    /// Tick colors use explicit <c>[color]</c> tags: the game's named tags do not apply inside table cells.
    /// </summary>
    private static string BuildTable(List<string> hostDisabled, List<string> localDisabled)
    {
        var table = new StringBuilder("[table=3]");
        AppendRow(table, Gold(Text("character")), Gold(Text("host")), Gold(Text("you")));
        foreach (var character in DisabledCharacterSync.Characters)
        {
            var hostOn = !hostDisabled.Contains(character);
            var localOn = !localDisabled.Contains(character);
            if (hostOn != localOn)
                AppendRow(table, character, Tick(hostOn), Tick(localOn));
        }

        return table.Append("[/table]").ToString();
    }

    /// <summary>The name column gets twice the share of each tick column, which is centered.</summary>
    private static void AppendRow(StringBuilder table, string name, string host, string local)
    {
        table.Append("[cell ratio=2 padding=8,0,8,0]").Append(name).Append("[/cell]");
        table.Append("[cell ratio=1 padding=8,0,8,0][center]").Append(host).Append("[/center][/cell]");
        table.Append("[cell ratio=1 padding=8,0,8,0][center]").Append(local).Append("[/center][/cell]");
    }

    private static string Gold(string text)
    {
        return "[gold]" + text + "[/gold]";
    }

    private static string Tick(bool enabled)
    {
        return enabled ? "[color=#6fd36f]✓[/color]" : "[color=#e0584f]✗[/color]";
    }

    private static string Text(string key)
    {
        return new LocString("settings_ui", "DOWNFALL-CHARACTER_MISMATCH.table." + key).GetFormattedText();
    }
}

/// <summary>Rewires the buttons of popups created by <see cref="CharacterMismatchPopupPatch" />.</summary>
[HarmonyPatch(typeof(NErrorPopup), nameof(NErrorPopup._Ready))]
internal static class CharacterMismatchPopupButtonsPatch
{
    [HarmonyPostfix]
    private static void Postfix(NErrorPopup __instance)
    {
        if (!CharacterMismatchPopupPatch.HostSettings.TryGetValue(__instance, out var pending)) return;

        var popup = __instance.GetNode<NVerticalPopup>("VerticalPopup");
        popup.DisconnectSignals();
        popup.InitYesButton(new LocString("settings_ui", "DOWNFALL-CHARACTER_MISMATCH.apply"), _ =>
        {
            DisabledCharacterSync.Apply(pending.Disabled);
            DisabledCharacterSync.RestartGame(pending.LobbyId);
        });
        popup.InitNoButton(new LocString("settings_ui", "DOWNFALL-CHARACTER_MISMATCH.cancel"), _ => { });
    }
}
