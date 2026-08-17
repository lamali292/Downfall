namespace Downfall.DownfallCode.DynamicVars;

using MegaCrit.Sts2.Core.Localization.DynamicVars;

public class FuncStringVar(string name, Func<string> value) : DynamicVar(name, 0M)
{
    public override string ToString() => value();
}