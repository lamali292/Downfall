using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Collector.CollectorCode.DynamicVars;

public class KindleVar : DynamicVar
{
    public KindleVar(decimal amount) : base("Kindle", amount)
    {
        this.WithTooltip();
    }
}