using Downfall.DownfallCode.Abstract;

namespace Collector.CollectorCode.CustomEnums;

public class CollectorTip(string name) : CustomStaticTip(name)
{
    public static readonly CollectorTip Kindle = new(nameof(Kindle));
}


