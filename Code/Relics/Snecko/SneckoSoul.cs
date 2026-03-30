using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Relics.Awakened;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Relics.Snecko;

[Pool(typeof(SneckoRelicPool))]
public class SneckoSoul : SneckoRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    
    public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<SuperSneckoSoul>();
}