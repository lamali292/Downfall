using Downfall.Code.Core.Guardian;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Downfall.Code.Events;

public interface IOnGuardianModeChange
{
    Task OnGuardianModeChange(Player player, GuardianModeModel oldMode, GuardianModeModel newMode);
}