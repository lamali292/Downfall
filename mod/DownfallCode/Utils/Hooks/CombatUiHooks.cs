using MegaCrit.Sts2.Core.Combat;

namespace Downfall.DownfallCode.Utils;

public static class CombatUiHooks
{
    private static readonly List<Action<CombatState>> handlers = new();

    public static void Register(Action<CombatState> handler)
    {
        handlers.Add(handler);
    }

    internal static void RaiseActivate(CombatState state)
    {
        foreach (var handler in handlers)
            try
            {
                handler(state);
            }
            catch (Exception e)
            {
                DownfallMainFile.Logger.Error($"CombatUi activate handler failed: {e}");
            }
    }
}