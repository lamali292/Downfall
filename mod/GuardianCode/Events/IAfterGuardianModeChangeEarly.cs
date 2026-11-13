using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Events;

public interface IAfterGuardianModeChangeEarly
{
    Task AfterGuardianModeChangeEarly(PlayerChoiceContext ctx, Player player, GuardianModeModel oldMode,
        GuardianModeModel newMode);
}