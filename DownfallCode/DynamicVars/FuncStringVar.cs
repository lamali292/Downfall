using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Downfall.DownfallCode.DynamicVars;

public class FuncStringVar(string name, Func<string> value) : DynamicVar(name, 0M)
{
    public override string ToString()
    {
        return value();
    }
}