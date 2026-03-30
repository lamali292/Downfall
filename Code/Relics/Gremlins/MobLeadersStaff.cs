using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Relics.Awakened;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Relics.Gremlins;

[Pool(typeof(GremlinsRelicPool))]
public class MobLeadersStaff : GremlinsRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<MobLeadersCrown>();
}