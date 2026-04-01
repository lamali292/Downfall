using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Automaton;

[Pool(typeof(AutomatonRelicPool))]
public class DonusWashers : AutomatonRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Shop;
    // TODO
}