using System.Threading.Tasks;
using Steamworks;
using MegaCrit.Sts2.Core.Platform.Steam;

namespace Downfall.DownfallCode.Voting;

public static class UserIdentity
{
    private static string? _id;
    private static bool IsAvailable => SteamInitializer.Initialized;

    public static string? Id
    {
        get
        {
            if (_id != null) return _id;
            if (!IsAvailable) return null;
            _id = SteamUser.GetSteamID().m_SteamID.ToString();
            return _id;
        }
    }

    private static TaskCompletionSource<string?>? _ticketTcs;
    private static Callback<GetTicketForWebApiResponse_t>? _cb;

    public static Task<string?> GetWebTicket()
    {
        if (!IsAvailable) return Task.FromResult<string?>(null);

        _ticketTcs = new TaskCompletionSource<string?>();
        _cb = Callback<GetTicketForWebApiResponse_t>.Create(OnTicket);
        SteamUser.GetAuthTicketForWebApi("votingservice");   // identity-string
        return _ticketTcs.Task;
    }

    private static void OnTicket(GetTicketForWebApiResponse_t r)
    {
        var hex = System.BitConverter.ToString(r.m_rgubTicket, 0, r.m_cubTicket).Replace("-", "");
        _ticketTcs?.TrySetResult(hex);
        _cb?.Dispose();
        _cb = null;
    }
}