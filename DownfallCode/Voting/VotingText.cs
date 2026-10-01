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
}
