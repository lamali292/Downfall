using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;

namespace Downfall.Code.Relics.Automaton;

[Pool(typeof(AwakenedRelicPool))]
public class BronzeCore : AutomatonRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
   
    public override RelicModel GetUpgradeReplacement() => ModelDb.Relic<PlatinumCore>();
    // TODO
}