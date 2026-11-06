using System.Text;
using Downfall.DownfallCode.Voting.Client;
using Godot;
using HttpClient = Godot.HttpClient;

namespace Downfall.DownfallCode.Voting;

/// <summary>
/// The real <see cref="IHttpAdapter"/>: one short-lived <see cref="HttpRequest"/>
/// node per call, parented to the scene root so callers don't need a node of
/// their own to host requests.
/// </summary>
public sealed class GodotHttpAdapter : IHttpAdapter
{
    public async Task<VotingResponse> SendAsync(
        Client.HttpVerb method, string url, IReadOnlyList<string> headers, byte[] body)
    {
        var root = ((SceneTree)Engine.GetMainLoop()).Root;

        var http = new HttpRequest();
        root.AddChild(http);

        var err = http.RequestRaw(url, headers.ToArray(), ToGodot(method), body);

        if (err != Error.Ok)
        {
            http.QueueFree();
            return new VotingResponse(0, $"request failed to send: {err}");
        }

        var result = await http.ToSignal(http, HttpRequest.SignalName.RequestCompleted);
        http.QueueFree();

        return new VotingResponse(result[1].AsInt64(), Encoding.UTF8.GetString(result[3].AsByteArray()));
    }

    private static HttpClient.Method ToGodot(Client.HttpVerb method) => method switch
    {
        Client.HttpVerb.Get => HttpClient.Method.Get,
        Client.HttpVerb.Post => HttpClient.Method.Post,
        Client.HttpVerb.Put => HttpClient.Method.Put,
        Client.HttpVerb.Delete => HttpClient.Method.Delete,
        _ => throw new ArgumentOutOfRangeException(nameof(method), method, null)
    };
}
