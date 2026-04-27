using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Rewards;
using MegaCrit.Sts2.Core.Rooms;

namespace Guardian.GuardianCode.Powers;

public class GemFinderPower : GuardianPowerModel
{
    public override Task AfterCombatEnd(CombatRoom room)
    {
        var player = Owner.Player;
        if (player == null) return Task.CompletedTask;
        var specialCardReward = new GemFinderReward(Amount, player);
        room.AddExtraReward(player, specialCardReward);
        return Task.CompletedTask;
    }
}