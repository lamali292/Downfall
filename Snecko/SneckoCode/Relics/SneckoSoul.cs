using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using Snecko.SneckoCode.Cards.Token;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Relics;

[Pool(typeof(SneckoRelicPool))]
public class SneckoSoul : SneckoRelicModel
{

    public SneckoSoul() : base(RelicRarity.Starter)
    {
        WithTip(typeof(SoulRoll));
    }
    
    
        
    public override RelicModel GetUpgradeReplacement()
    {
        return ModelDb.Relic<SuperSneckoSoul>();
    }
    // TODO
}