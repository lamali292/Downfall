using System.Security.Cryptography;
using System.Text;
using MegaCrit.Sts2.Core.Platform.Steam;
using Steamworks;

namespace Downfall.DownfallCode.Voting;

public static class UserIdentity
{
    private static TaskCompletionSource<string?>? _ticketTcs;
    private static Callback<GetTicketForWebApiResponse_t>? _cb;
    private static bool IsAvailable => SteamInitializer.Initialized;

    public static string? Id
    {
        get
        {
            if (field != null) return field;
            if (!IsAvailable) return null;
            var steamId = SteamUser.GetSteamID().m_SteamID.ToString();

            field = Convert.ToHexString(
                SHA256.HashData(Encoding.UTF8.GetBytes(steamId))
            )[..16];

            return field;
        }
    }

    public static Task<string?> GetWebTicket()
    {
        if (!IsAvailable) return Task.FromResult<string?>(null);

        _ticketTcs = new TaskCompletionSource<string?>();
        _cb = Callback<GetTicketForWebApiResponse_t>.Create(OnTicket);
        SteamUser.GetAuthTicketForWebApi("votingservice"); // identity-string
        return _ticketTcs.Task;
    }

    private static void OnTicket(GetTicketForWebApiResponse_t r)
    {
        var hex = Convert.ToHexString(r.m_rgubTicket, 0, r.m_cubTicket);
        _ticketTcs?.TrySetResult(hex);
        _cb?.Dispose();
        _cb = null;
    }
}