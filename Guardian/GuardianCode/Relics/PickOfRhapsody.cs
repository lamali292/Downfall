using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Rewards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Rooms;

namespace Guardian.GuardianCode.Relics;

[Pool(typeof(GuardianRelicPool))]
public class PickOfRhapsody() : GuardianRelicModel(RelicRarity.Uncommon)
{
    
    public override Task AfterCombatEnd(CombatRoom room)
    {
        if (room.RoomType != RoomType.Elite) return Task.CompletedTask;
        var gemReward = new GemFinderReward(1, Owner);
        room.AddExtraReward(Owner, gemReward);
        return Task.CompletedTask;
    }
}