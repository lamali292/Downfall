using Automaton.AutomatonCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Automaton.AutomatonCode.Relics;

[Pool(typeof(AutomatonRelicPool))]
public class DonusWashers : AutomatonRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    // TODO
}