using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;

namespace Downfall.DownfallCode.Utils;

// Lib mod
public static class DeathHooks
{
    // Return a Task to take over the death entirely; return null to pass.
    public delegate Task? DeathInterceptor(Creature creature);

    private static readonly List<DeathInterceptor> interceptors = new();

    public static void RegisterInterceptor(DeathInterceptor interceptor)
        => interceptors.Add(interceptor);

    internal static Task? TryIntercept(Creature creature)
    {
        foreach (var interceptor in interceptors)
        {
            try
            {
                var task = interceptor(creature);
                if (task != null) return task;   // first taker wins
            }
            catch (Exception e)
            {
                DownfallMainFile.Logger.Error($"Death interceptor failed: {e}");
            }
        }
        return null;
    }
}

