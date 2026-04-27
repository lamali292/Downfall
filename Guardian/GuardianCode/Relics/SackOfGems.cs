using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Guardian.GuardianCode.Relics;

[Pool(typeof(GuardianRelicPool))]
public class SackOfGems : GuardianRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    // TODO
}