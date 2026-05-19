using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Hermit.HermitCode.Utils;

public static class HermitCombatFx
{
    private const string GunHitVfx = "vfx/vfx_attack_blunt";
    private const string FireHitVfx = "vfx/vfx_fire_burst";
    private const string BluntHitVfx = "vfx/vfx_attack_blunt";
    private const string ShortFuseHitVfx = "vfx/vfx_coin_explosion_small";
    private const string SlashHitVfx = "vfx/vfx_attack_slash";

    public static void GroundFireOnTarget(Creature target)
    {
        var vfx = NGroundFireVfx.Create(target);
        if (vfx != null && NCombatRoom.Instance != null) NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
        SfxCmd.Play("event:/sfx/characters/attack_fire");
    }

    public static AttackCommand WithHermitGunHitFx(this AttackCommand cmd)
    {
        return cmd.WithHitFx(GunHitVfx);
    }

    public static AttackCommand WithHermitFireHitFx(this AttackCommand cmd)
    {
        return cmd.WithHitFx(FireHitVfx, "event:/sfx/characters/attack_fire");
    }

    public static AttackCommand WithHermitBluntLightHitFx(this AttackCommand cmd)
    {
        return cmd.WithHitFx(BluntHitVfx, null, "blunt_attack.mp3");
    }

    public static AttackCommand WithHermitBluntHeavyHitFx(this AttackCommand cmd)
    {
        return cmd.WithHitFx(BluntHitVfx, null, "heavy_attack.mp3");
    }

    public static AttackCommand WithHermitShortFuseHitFx(this AttackCommand cmd)
    {
        return cmd.WithHitFx(ShortFuseHitVfx, null, "heavy_attack.mp3");
    }

    public static AttackCommand WithHermitSlashHitFx(this AttackCommand cmd)
    {
        return cmd.WithHitFx(SlashHitVfx, null, "slash_attack.mp3");
    }
}