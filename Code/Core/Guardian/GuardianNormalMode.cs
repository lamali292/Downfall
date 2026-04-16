namespace Downfall.Code.Core.Guardian;

public class GuardianNormalMode : GuardianModeModel
{
    public override bool ShouldReceiveCombatHooks => true;
}