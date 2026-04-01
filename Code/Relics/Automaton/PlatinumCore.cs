using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Automaton;

[Pool(typeof(AwakenedRelicPool))]
public class PlatinumCore : AutomatonRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    // TODO
}