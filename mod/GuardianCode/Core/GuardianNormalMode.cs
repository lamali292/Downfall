namespace Guardian.GuardianCode.Core;

public class GuardianNormalMode : GuardianModeModel
{
    public override bool ShouldReceiveCombatHooks => true;
}