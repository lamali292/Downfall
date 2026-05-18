using BaseLib.Audio;

namespace HermitMod.Utility;

public static class HermitSfx
{
    public const string Gun1 = "res://Hermit/audio/hermit_gun.ogg";
    public const string Gun2 = "res://Hermit/audio/hermit_gun2.ogg";
    public const string Gun3 = "res://Hermit/audio/hermit_gun3.ogg";
    public const string Spin = "res://Hermit/audio/hermit_spin.ogg";
    public const string Reload = "res://Hermit/audio/hermit_reload.ogg";
    public const float DefaultDb = 5f;
    public const float SpinPitchVariation = 0.15f;
    public const float GunPitchVariation = 0.1f;

    public static void PlayGun1(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
    {
        ModAudio.PlaySound(Gun1, volumeDb, pitchVariation);
    }

    public static void PlayGun2(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
    {
        ModAudio.PlaySound(Gun2, volumeDb, pitchVariation);
    }

    public static void PlayGun3(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
    {
        ModAudio.PlaySound(Gun3, volumeDb, pitchVariation);
    }

    public static void PlaySpin(float volumeDb = DefaultDb, float pitchVariation = SpinPitchVariation)
    {
        ModAudio.PlaySound(Spin, volumeDb, pitchVariation);
    }

    public static void PlayReload(float volumeDb = DefaultDb, float pitchVariation = GunPitchVariation)
    {
        ModAudio.PlaySound(Reload, volumeDb, pitchVariation);
    }
}