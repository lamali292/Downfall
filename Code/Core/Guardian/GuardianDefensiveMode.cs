namespace Downfall.Code.Core.Guardian;

public class GuardianDefensiveMode : GuardianModeModel
{
    public override bool ShouldReceiveCombatHooks => true;
}