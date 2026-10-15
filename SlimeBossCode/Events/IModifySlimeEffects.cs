using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Events;

public interface IModifySlimeEffects
{
    int ModifySlimeEffects(int amount, SlimeModel slime);
}