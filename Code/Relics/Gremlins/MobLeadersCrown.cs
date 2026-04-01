using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Gremlins;

[Pool(typeof(GremlinsRelicPool))]
public class MobLeadersCrown : GremlinsRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    // TODO
}