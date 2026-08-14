using BaseLib.Audio;
using Downfall.DownfallCode.Utils.Sound;
using MegaCrit.Sts2.Core.Commands;

namespace Hermit.HermitCode.Utils;

public static class HermitSfx
{
    public static void PlayGun1()
    {
        SfxCmd.Play("event:/sfx/characters/hermit-hermit/hermit-hermit_gun1");
    }

    public static void PlayGun2()
    {
        SfxCmd.Play("event:/sfx/characters/hermit-hermit/hermit-hermit_gun2");
    }

    public static void PlayGun3()
    {
        SfxCmd.Play("event:/sfx/characters/hermit-hermit/hermit-hermit_gun3");
    }

    public static void PlaySpin()
    {
        SfxCmd.Play("event:/sfx/characters/hermit-hermit/hermit-hermit_spin");
    }

    public static void PlayReload()
    {
        SfxCmd.Play("event:/sfx/characters/hermit-hermit/hermit-hermit_reload");
    }
}