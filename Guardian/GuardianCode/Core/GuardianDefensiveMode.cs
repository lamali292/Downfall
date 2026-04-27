namespace Guardian.GuardianCode.Core;

public class GuardianDefensiveMode : GuardianModeModel
{
    public override bool ShouldReceiveCombatHooks => true;
}