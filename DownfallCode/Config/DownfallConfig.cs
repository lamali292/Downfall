using BaseLib.Config;

namespace Downfall.DownfallCode.Config;

[ConfigHoverTipsByDefault]
internal class DownfallConfig : SimpleModConfig
{
    public static bool UploadMetrics { get; set; } = false;
}