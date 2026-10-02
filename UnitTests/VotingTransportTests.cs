using Downfall.DownfallCode.Voting.Client;
using Xunit;

namespace Downfall.UnitTests;

public class VotingTransportTests
{
    private sealed class ScriptedHttp(params long[] statuses) : IHttpAdapter
    {
        public List<IReadOnlyList<string>> Sent { get; } = [];

        public Task<VotingResponse> SendAsync(HttpVerb method, string url, IReadOnlyList<string> headers, byte[] body)
        {
            var status = statuses[Math.Min(Sent.Count, statuses.Length - 1)];
            Sent.Add(headers);
            return Task.FromResult(new VotingResponse(status, ""));
        }
    }

    private sealed class FakeAuth(bool reauthSucceeds) : IAuthTokens
    {
        public int Reauths { get; private set; }
        public string? Token { get; private set; } = "old";

        public Task<bool> ReauthenticateAsync()
        {
            Reauths++;
            if (reauthSucceeds)
                Token = "new";
            return Task.FromResult(reauthSucceeds);
        }
    }

    private static (VotingTransport Transport, ScriptedHttp Http, FakeAuth Auth) Create(bool reauthSucceeds, params long[] statuses)
    {
        var http = new ScriptedHttp(statuses);
        var auth = new FakeAuth(reauthSucceeds);
        return (new VotingTransport(http, "https://x", "key") { Auth = auth }, http, auth);
    }

    [Theory]
    [InlineData(HttpVerb.Post, "/vote")]
    [InlineData(HttpVerb.Put, "/my/profile")]
    [InlineData(HttpVerb.Post, "/submissions")]
    [InlineData(HttpVerb.Get, "/my/submissions")]
    [InlineData(HttpVerb.Delete, "/my/submissions/3")]
    public async Task AuthedRequestRetriesOnceWithNewToken(HttpVerb verb, string path)
    {
        var (transport, http, auth) = Create(true, 401, 200);

        var response = await transport.SendAsync(new VotingRequest(verb, path) { Authed = true });

        Assert.Equal(200, response.Status);
        Assert.Equal(1, auth.Reauths);
        Assert.Equal(2, http.Sent.Count);
        Assert.Contains("Authorization: Bearer old", http.Sent[0]);
        Assert.Contains("Authorization: Bearer new", http.Sent[1]);
    }

    [Fact]
    public async Task SecondUnauthorizedIsReturnedNotRetriedAgain()
    {
        var (transport, http, auth) = Create(true, 401, 401, 200);

        var response = await transport.SendAsync(new VotingRequest(HttpVerb.Post, "/vote") { Authed = true });

        Assert.Equal(401, response.Status);
        Assert.Equal(1, auth.Reauths);
        Assert.Equal(2, http.Sent.Count);
    }

    [Fact]
    public async Task FailedReauthReturnsOriginal401WithoutResend()
    {
        var (transport, http, _) = Create(false, 401, 200);

        var response = await transport.SendAsync(new VotingRequest(HttpVerb.Post, "/vote") { Authed = true });

        Assert.Equal(401, response.Status);
        Assert.Single(http.Sent);
    }

    [Fact]
    public async Task OptedOutRequestNeverReauthenticates()
    {
        var (transport, http, auth) = Create(true, 401, 200);

        var response = await transport.SendAsync(
            new VotingRequest(HttpVerb.Get, "/my/profile") { Authed = true, RetryOnUnauthorized = false });

        Assert.Equal(401, response.Status);
        Assert.Equal(0, auth.Reauths);
        Assert.Single(http.Sent);
    }

    [Fact]
    public async Task AnonymousRequestNeverReauthenticatesOrSendsToken()
    {
        var (transport, http, auth) = Create(true, 401, 200);

        var response = await transport.SendAsync(new VotingRequest(HttpVerb.Get, "/feed"));

        Assert.Equal(401, response.Status);
        Assert.Equal(0, auth.Reauths);
        Assert.DoesNotContain(http.Sent[0], h => h.StartsWith("Authorization"));
    }
}
