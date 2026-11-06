using Downfall.DownfallCode.Config;
using MegaCrit.Sts2.Core.Context;

namespace Downfall.DownfallCode.Patches.KaleidoscopePatch;

static class PrismaticModeConfigSync
{
    private static readonly Dictionary<ulong, PrismaticMode> ByOwner = new();
    private static System.Action? _rebroadcast;
    private static readonly Lock Lock = new();
    public static void SetRebroadcast(Action? f) => _rebroadcast = f;

    public static void Reset()
    {
        lock (Lock) ByOwner.Clear();
    }

    public static void CaptureLocal()
    {
        if (LocalContext.NetId is not { } me) return;
        lock (Lock)
        {
            ByOwner[me] = DownfallConfig.PrismaticOption;
        }
    }

    public static void OnConfig(PrismaticConfigMessage msg, ulong senderId)
    {
        lock (Lock)
        {
            var isNew = !ByOwner.ContainsKey(msg.OwnerNetId);
            ByOwner[msg.OwnerNetId] = msg.PrismaticMode;
            if (isNew && LocalContext.NetId is { } me && msg.OwnerNetId != me)
                _rebroadcast?.Invoke();
        }
    }

    public static PrismaticMode For(ulong ownerId)
    {
        lock (Lock) return ByOwner.GetValueOrDefault(ownerId, PrismaticMode.All);
    }
}