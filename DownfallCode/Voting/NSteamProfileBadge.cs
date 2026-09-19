using Godot;
using MegaCrit.Sts2.Core.Helpers;
using Steamworks;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// "You're signed in as ..." readout - the avatar and persona name of the
/// account actually backing the current <see cref="VotingAuth"/> session
/// (see <see cref="SteamAvatar"/>), not just whichever Steam account
/// happens to be running the game locally. Hidden entirely until there's a
/// real signed-in session to show, and refreshes reactively via
/// <see cref="VotingAuth.SignedIn"/> for popups (like the upload flow) that
/// sign in lazily well after this badge's own <c>_Ready()</c>. Its own
/// scene since it's reused across the upload popup and My Submissions popup.
/// </summary>
public partial class NSteamProfileBadge : HBoxContainer
{
    public const string ScenePath = "res://Downfall/scenes/voting/steam_profile_badge.tscn";

    private TextureRect _avatar = null!;
    private Label _nameLabel = null!;

    public override void _Ready()
    {
        _avatar = GetNode<TextureRect>("%Avatar");
        _nameLabel = GetNode<Label>("%NameLabel");
        Visible = false;

        VotingAuth.SignedIn += OnSignedIn;
        TaskHelper.RunSafely(Refresh());
    }

    public override void _ExitTree()
    {
        VotingAuth.SignedIn -= OnSignedIn;
    }

    private void OnSignedIn() => TaskHelper.RunSafely(Refresh());

    private async Task Refresh()
    {
        if (!VotingAuth.IsSignedIn)
            return;

        var steamIdText = await VotingApi.Instance.GetMySteamId();
        if (!IsInstanceValid(this) || steamIdText == null || !ulong.TryParse(steamIdText, out var raw))
            return;

        var steamId = new CSteamID(raw);

        var name = await SteamAvatar.GetPersonaNameAsync(steamId);
        if (!IsInstanceValid(this))
            return;

        _nameLabel.Text = name ?? "";
        Visible = true;

        var texture = await SteamAvatar.GetAvatarAsync(steamId);
        if (IsInstanceValid(this) && texture != null)
            _avatar.Texture = texture;
    }
}
