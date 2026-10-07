using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Rewards;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Guardian.GuardianCode.Relics;

[Pool(typeof(GuardianRelicPool))]
public class SackOfGems : GuardianRelicModel
{
    public SackOfGems() : base(RelicRarity.Shop)
    {
        WithVar("Gem", 5);
        WithVar("MaxGems", 10);
    }

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        await RewardsCmd.OfferCustom(Owner, 
            [
                new GemFinderReward(
                DynamicVars["Gem"].IntValue, 
                DynamicVars["MaxGems"].IntValue, 
                Owner
                )
            ]);
    }
}