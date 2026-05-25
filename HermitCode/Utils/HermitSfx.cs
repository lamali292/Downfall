using BaseLib.Audio;
using Downfall.DownfallCode.Utils.Sound;

namespace Hermit.HermitCode.Utils;

public static class HermitSfx
{
    private const string Gun1 = "res://Hermit/audio/hermit_gun.ogg";
    private const string Gun2 = "res://Hermit/audio/hermit_gun2.ogg";
    private const string Gun3 = "res://Hermit/audio/hermit_gun3.ogg";
    private const string Spin = "res://Hermit/audio/hermit_spin.ogg";
    private const string Reload = "res://Hermit/audio/hermit_reload.ogg";
    private const float DefaultDb = 5f;
    private const float SpinPitchVariation = 0.15f;
    private const float GunPitchVariation = 0.1f;

    public static void PlayGun1(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
    {
        MyModAudio.PlaySound(Gun1, volumeDb, pitchVariation);
    }

    public static void PlayGun2(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
    {
        MyModAudio.PlaySound(Gun2, volumeDb, pitchVariation);
    }

    public static void PlayGun3(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
    {
        MyModAudio.PlaySound(Gun3, volumeDb, pitchVariation);
    }

    public static void PlaySpin(float volumeDb = DefaultDb, float pitchVariation = SpinPitchVariation)
    {
        MyModAudio.PlaySound(Spin, volumeDb, pitchVariation);
    }

    public static void PlayReload(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
    {
        MyModAudio.PlaySound(Reload, volumeDb, pitchVariation);
    }
}