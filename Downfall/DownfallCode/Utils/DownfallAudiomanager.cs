using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Audio;
using System.IO;

namespace Downfall.DownfallCode.Utils;

public class DownfallAudiomanager
{
    public static void LoadFModBank(string id) { }
}


/*

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager._EnterTree))]
internal static class NAudioManagerEnterTreePatch
{
    private static bool _banksLoaded = false;
    private static Godot.Variant _downfallBank;

    [HarmonyPostfix]
    private static void AfterEnterTree()
    {
        if (_banksLoaded) return;
        _banksLoaded = true;

        var server = Engine.GetSingleton("FmodServer");
        _downfallBank = server.Call("load_bank", "res://Downfall/banks/Downfall.bank", 0);
    
        // Pre-load sample data for all events in the bank
        var bankObj = _downfallBank.Obj as GodotObject;
        var descList = bankObj.Call("get_description_list").AsGodotArray();
        foreach (var item in descList)
        {
            var d = item.Obj as GodotObject;
            d?.Call("load_sample_data");
        }

        // Block until everything is in memory
        server.Call("wait_for_all_loads");
        GD.Print("[DownfallAudio] All sample data loaded");
    }
}

[HarmonyPatch(typeof(NAudioManager), nameof(NAudioManager.PlayOneShot), new[] { typeof(string), typeof(float) })]
internal static class PlayOneShotPatch
{
    [HarmonyPrefix]
    private static bool Prefix(string path, float volume)
    {
        
        if (path == "event:/selection/selection_gremlins")
        {
            var server = Engine.GetSingleton("FmodServer");

// Step 1: does the server know about this GUID at all?
            var desc = server.Call("get_event_from_guid", "{75f05464-13c6-48de-8e8d-72aaefd39c55}");
            var descObj = desc.Obj as GodotObject;

// Load sample data explicitly first
            descObj.Call("load_sample_data");

// Wait a moment for it to load, then create instance
            var inst = server.Call("create_event_instance_from_description", desc).Obj as GodotObject;
            inst?.Call("start");

            GD.Print("[DownfallAudio] Playback state: " + inst?.Call("get_playback_state"));
// 0 = PLAYING, 2 = STOPPED (stopped immediately = no audio data)

            inst?.Call("release");
            return false;
        }
        return true;
        if (path == "event:/selection/selection_gremlins")
        {
            
            var s = Engine.GetSingleton("FmodServer");
            // Test with a known game sound first
            s.Call("play_one_shot", "event:/sfx/buff");
            return false;
            
            var s = Engine.GetSingleton("FmodServer");
            s.Call("play_one_shot_using_guid", "{75f05464-13c6-48de-8e8d-72aaefd39c55}");
            return false;
        }
        return true;
    }
}

*/