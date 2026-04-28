using Downfall.DownfallCode.Abstract;

namespace Guardian.GuardianCode.CustomEnums;

public class GuardianTip(string name) : CustomStaticTip(name)
{
    public static readonly GuardianTip Accelerate = new(nameof(Accelerate));
    public static readonly GuardianTip Stasis = new(nameof(Stasis));
    public static readonly GuardianTip Brace = new(nameof(Brace));
    public static readonly GuardianTip Tick = new(nameof(Tick));
}

