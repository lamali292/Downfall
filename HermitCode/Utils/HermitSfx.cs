using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.TestSupport;

namespace Hermit.HermitCode.Utils;

public static class HermitSfx
{
    public static void PlayGun1()
    {
        if (TestMode.IsOff) SfxCmd.Play("event:/sfx/characters/hermit-hermit/hermit-hermit_gun1");
    }

    public static void PlayGun2()
    {
        if (TestMode.IsOff) SfxCmd.Play("event:/sfx/characters/hermit-hermit/hermit-hermit_gun2");
    }

    public static void PlayGun3()
    {
        if (TestMode.IsOff) SfxCmd.Play("event:/sfx/characters/hermit-hermit/hermit-hermit_gun3");
    }

    public static void PlaySpin()
    {
        if (TestMode.IsOff) SfxCmd.Play("event:/sfx/characters/hermit-hermit/hermit-hermit_spin");
    }

    public static void PlayReload()
    {
        if (TestMode.IsOff) SfxCmd.Play("event:/sfx/characters/hermit-hermit/hermit-hermit_reload");
    }
}