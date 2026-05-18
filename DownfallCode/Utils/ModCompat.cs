using MegaCrit.Sts2.Core.Modding;

namespace Downfall.DownfallCode.Utils;

public class ModCompat
{
    public static async Task TryExecute(string modId, Func<Task> action)
    {
        var isLoaded = ModManager.GetLoadedMods()
            .Any(m => m.manifest?.id == modId);
        if (!isLoaded) return;
        await action();
    }
}