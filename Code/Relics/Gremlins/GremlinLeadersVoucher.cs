using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Gremlins;

[Pool(typeof(GremlinsRelicPool))]
public class GremlinLeadersVoucher : GremlinsRelicModel
{
    // TODO - Boss relic
    public override RelicRarity Rarity => RelicRarity.Ancient;
    // TODO
}