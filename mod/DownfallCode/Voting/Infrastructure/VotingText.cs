using Downfall.DownfallCode.Voting.Client;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// The only place that turns the voting client's outcome and error codes into
/// player-facing (localized) text; the client and session never format strings.
/// </summary>
public static class VotingText
{
    public static string ForLogin(LoginOutcome outcome) => outcome switch
    {
        LoginOutcome.Success => VotingUi.Loc("DOWNFALL-VOTING.status_login_success"),
        LoginOutcome.Unreachable => VotingUi.Loc("DOWNFALL-VOTING.error_login_unreachable"),
        LoginOutcome.Banned => VotingUi.Loc("DOWNFALL-VOTING.error_login_banned"),
        LoginOutcome.Expired => VotingUi.Loc("DOWNFALL-VOTING.error_login_expired"),
        LoginOutcome.Timeout => VotingUi.Loc("DOWNFALL-VOTING.error_login_timeout"),
        _ => VotingUi.Loc("DOWNFALL-VOTING.error_login_generic"),
    };

    /// <summary>
    /// <paramref name="genericKey"/> is the loc key used when the server gave no
    /// message of its own; keys taking a <c>{code}</c> get the HTTP status.
    /// </summary>
    public static string For(VotingError error, string genericKey) => error.Code switch
    {
        VotingErrorCode.NoIdentity or VotingErrorCode.NotSignedIn =>
            VotingUi.Loc("DOWNFALL-VOTING.error_upload_not_signed_in"),
        VotingErrorCode.LoginUnreachable => ForLogin(LoginOutcome.Unreachable),
        VotingErrorCode.LoginBanned or VotingErrorCode.Banned => ForLogin(LoginOutcome.Banned),
        VotingErrorCode.LoginExpired => ForLogin(LoginOutcome.Expired),
        VotingErrorCode.LoginTimeout => ForLogin(LoginOutcome.Timeout),
        VotingErrorCode.LoginFailed => ForLogin(LoginOutcome.Failed),
        VotingErrorCode.SessionExpired => VotingUi.Loc("DOWNFALL-VOTING.error_upload_session_expired"),
        VotingErrorCode.TooLarge => VotingUi.Loc("DOWNFALL-VOTING.error_upload_too_large"),
        VotingErrorCode.UnsupportedImageType => VotingUi.Loc("DOWNFALL-VOTING.error_unsupported_type"),
        VotingErrorCode.Network => ForLogin(LoginOutcome.Unreachable),
        _ => error.ServerMessage ?? VotingUi.Loc(genericKey, ("code", error.Status.ToString())),
    };
}
