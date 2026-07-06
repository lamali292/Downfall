using BaseLib.Config;

namespace Downfall.DownfallCode.Config;

[ConfigHoverTipsByDefault]
public class DownfallConfig : SimpleModConfig
{
    public static bool UploadMetrics { get; set; } = false;
    public static bool DevMode { get; set; } = false;
}