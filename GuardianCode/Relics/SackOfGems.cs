using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Rewards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Guardian.GuardianCode.Relics;

[Pool(typeof(GuardianRelicPool))]
public class SackOfGems() : GuardianRelicModel(RelicRarity.Shop)
{
    public override async Task AfterObtained()
    {
        await RewardsCmd.OfferCustom(Owner, [new GemFinderReward(5, 10, Owner)]);
    }
}