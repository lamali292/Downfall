using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Runs;

namespace Downfall.DownfallCode.Utils;

public static class RunHooks
{
    private static readonly List<Action<RunState>> newRunHandlers = new();

    public static void OnNewRun(Action<RunState> handler)
    {
        newRunHandlers.Add(handler);
    }

    public static void OnNewRunPerPlayer(Action<Player> handler)
    {
        newRunHandlers.Add(state =>
        {
            foreach (var player in state.Players) handler(player);
        });
    }

    internal static void RaiseNewRun(RunState state)
    {
        foreach (var handler in newRunHandlers)
            try
            {
                handler(state);
            }
            catch (Exception e)
            {
                DownfallMainFile.Logger.Error($"New-run handler failed: {e}");
            }
    }
}