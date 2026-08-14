using Godot;

namespace Downfall.DownfallCode.Audio;

internal static class FmodStudioGateway
{
    internal static readonly StringName ServerName = new("FmodServer");

    public static GodotObject? TryGetServer()
    {
        try
        {
            if (!Engine.HasSingleton(ServerName))
                return null;

            var server = Engine.GetSingleton(ServerName);
            return server is not null && GodotObject.IsInstanceValid(server) ? server : null;
        }
        catch (Exception ex)
        {
            DownfallMainFile.Logger.Error($"[Audio] FmodServer singleton: {ex}");
            return null;
        }
    }

    public static bool TryCall(out Variant result, StringName method, params Variant[] args)
    {
        result = default;
        var server = TryGetServer();
        if (server is null || !server.HasMethod(method))
            return false;

        try
        {
            result = args.Length == 0 ? server.Call(method) : server.Call(method, args);
            return true;
        }
        catch (Exception ex)
        {
            DownfallMainFile.Logger.Error($"[Audio] FMOD {method}: {ex}");
            return false;
        }
    }

    public static bool TryCall(StringName method, params Variant[] args)
    {
        return TryCall(out _, method, args);
    }
}