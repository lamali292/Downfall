using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Downfall.Code.Vfx;

public static class SfxPlayer
{
    public static void PlaySfx(string path, float pitch = 1.0f, float volume = 1.0f)
    {
        if (NonInteractiveMode.IsActive) return;

        AudioStream stream;
        try
        {
            stream = PreloadManager.Cache.GetAsset<AudioStream>(path);
        }
        catch
        {
            GD.PrintErr($"[SfxPlayer] Could not load audio: {path}");
            return;
        }

        var audioPlayer = new AudioStreamPlayer();
        audioPlayer.Stream = stream;
        audioPlayer.Bus = "SFX";

        audioPlayer.PitchScale = pitch;
        audioPlayer.VolumeDb = Mathf.LinearToDb(volume);

        audioPlayer.Finished += () => audioPlayer.QueueFree();

        var root = (Node?)NCombatRoom.Instance ?? NGame.Instance;
        if (root != null)
        {
            root.AddChild(audioPlayer);
            audioPlayer.Play();
        }
        else
        {
            audioPlayer.QueueFree();
        }
    }
}