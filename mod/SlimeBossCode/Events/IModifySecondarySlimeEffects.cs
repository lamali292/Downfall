using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Events;

public interface IModifySecondarySlimeEffects
{
    int ModifySecondarySlimeEffects(int amount, SlimeModel slime);
}