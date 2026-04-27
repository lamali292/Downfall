using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class ShreddedDoll : AwakenedRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    // TODO
}