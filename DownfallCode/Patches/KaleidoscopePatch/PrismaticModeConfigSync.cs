using Downfall.DownfallCode.Config;
using MegaCrit.Sts2.Core.Context;

namespace Downfall.DownfallCode.Patches.KaleidoscopePatch;

static class PrismaticModeConfigSync
{
    private static readonly Dictionary<ulong, PrismaticMode> ByOwner = new();
    private static System.Action? _rebroadcast;
    public static void SetRebroadcast(Action? f) => _rebroadcast = f;

    public static void Reset() => ByOwner.Clear();

    public static void CaptureLocal()
    {
        if (LocalContext.NetId is { } me)
            ByOwner[me] = DownfallConfig.PrismaticOption;
    }

    public static void OnConfig(PrismaticConfigMessage msg, ulong senderId)
    {
         var isNew = !ByOwner.ContainsKey(msg.OwnerNetId);
        ByOwner[msg.OwnerNetId] = msg.PrismaticMode;
        
        if (isNew && LocalContext.NetId is { } me && msg.OwnerNetId != me)
            _rebroadcast?.Invoke();
    }

    public static PrismaticMode For(ulong ownerId)
        => ByOwner.GetValueOrDefault(ownerId, PrismaticMode.All);
}