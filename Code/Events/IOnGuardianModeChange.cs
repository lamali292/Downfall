using Downfall.Code.Core.Guardian;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Events;

public interface IOnGuardianModeChange
{
    Task OnGuardianModeChange(Player player, GuardianModeModel oldMode, GuardianModeModel newMode);
}