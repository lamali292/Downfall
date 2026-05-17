

//namespace Downfall.DownfallCode.Utils;

/*
public static class DownfallAudiomanager
{
    public static GodotObject _fmodServer;
    private static readonly List<string> _bankPaths = new(); // registry for reload

    public static void LoadFModBank(string modId)
    {
        //
        if (!modId.Equals("Downfall")) return;
        LoadBankFile($"res://{modId}/banks/{modId}.strings.bank");
        LoadBankFile($"res://{modId}/banks/{modId}.bank");
    }

    public static void PlayOneShot(string eventPath)
    {
        NAudioManager.Instance?.PlayOneShot(eventPath);
    }

    public static void ForceReloadBanks()
    {
        if (_fmodServer == null) return;

        var pathsToReload = new List<string>(_bankPaths);
        _bankPaths.Clear();

        foreach (var path in pathsToReload)
            LoadBankFile(path);

        GD.Print($"[DownfallAudio] Reloaded {pathsToReload.Count} bank files.");
    }

    public static void EnsureBanksLoaded()
    {
        if (_fmodServer == null || _bankPaths.Count == 0) return;

        // Quick check using the first bank path
        var testPath = _bankPaths[0];
        // Try to get any event — if it fails, banks were dropped
        var test = _fmodServer.Call("check_event_path", "event:/selection/selection_gremlins").AsBool();
        if (test) return;
        GD.Print("[DownfallAudio] Banks dropped — reloading...");
        ForceReloadBanks();
    }

    private static void LoadBankFile(string path)
    {
        if (_bankPaths.Contains(path))
        {
            GD.Print($"[DownfallAudio] Already loaded, skipping: {path}");
            return;
        }

        var bank = _fmodServer.Call("load_bank", path, 0).Obj as GodotObject;
        if (bank == null)
        {
            GD.PrintErr($"[DownfallAudio] Failed to load: {path}");
            return;
        }

        var descriptions = bank.Call("get_description_list").AsGodotArray();

        // After banks load, check bus existence
        var sfxBus = _fmodServer.Call("get_bus", "bus:/master/sfx").Obj as GodotObject;
        var masterBus = _fmodServer.Call("get_bus", "bus:/master").Obj as GodotObject;
        GD.Print($"[DownfallAudio] bus:/master/sfx exists: {sfxBus != null}");
        GD.Print($"[DownfallAudio] bus:/master exists: {masterBus != null}");

        if (sfxBus != null)
        {
            var vol = sfxBus.Get("volume");
            var muted = sfxBus.Get("mute");
            var paused = sfxBus.Get("paused");
            GD.Print($"[DownfallAudio] sfx bus — volume: {vol} | muted: {muted} | paused: {paused}");
        }

        if (masterBus != null)
        {
            var vol = masterBus.Get("volume");
            var muted = masterBus.Get("mute");
            GD.Print($"[DownfallAudio] master bus — volume: {vol} | muted: {muted}");
        }

        var desc = _fmodServer.Call("get_event", "event:/selection/selection_gremlins").Obj as GodotObject;
        if (desc != null)
        {
            var inst = _fmodServer.Call("create_event_instance_from_description", desc).Obj as GodotObject;
            if (inst != null)
            {
                // Get the channel group this instance outputs to
                var channelGroup = inst.Call("get_channel_group").Obj as GodotObject;
                GD.Print($"[DownfallAudio] channel group: {channelGroup != null}");
                if (channelGroup != null)
                {
                    var groupName = channelGroup.Call("get_name");
                    GD.Print($"[DownfallAudio] channel group name: {groupName}");
                }

                // Also check the bus this event description targets
                var userProps = desc.Call("get_user_property_count");
                GD.Print($"[DownfallAudio] user property count: {userProps}");

                inst.Call("release");
            }
        }

        _bankPaths.Add(path);
        GD.Print($"[DownfallAudio] Loaded: {path} ({descriptions.Count} events)");
    }
}
/*
[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager._EnterTree))]
internal static class NAudioManagerEnterTreePatch
{
    private static bool _initialized = false;

    [HarmonyPostfix]
    private static void AfterEnterTree()
    {
        if (_initialized) return;
        _initialized = true;

        DownfallAudiomanager._fmodServer = Engine.GetSingleton("FmodServer") as GodotObject;
        DownfallAudiomanager.LoadFModBank("Downfall");
        GD.Print("[DownfallAudio] Ready");
    }
}

[HarmonyPatch(typeof(NGame), nameof(NGame.LogResourceStats))]
internal static class LogResourceStatsPatch
{
    [HarmonyPostfix]
    private static void Postfix(string context)
    {
        if (context == "main menu loaded (complete)")
        {
            GD.Print("[DownfallAudio] Reloading mod banks after preload...");
            DownfallAudiomanager.ForceReloadBanks();
        }
    }
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot),
    new[] { typeof(string), typeof(System.Collections.Generic.Dictionary<string, float>), typeof(float) })]
internal static class PlayOneShotEnsurePatch
{
    [HarmonyPrefix]
    private static void Prefix()
    {
        DownfallAudiomanager.EnsureBanksLoaded();
    }
}*/