using Awakened.AwakenedCode.Core;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Awakened.AwakenedCode.Relics;

[Pool(typeof(AwakenedRelicPool))]
public class TomeOfPortalmancy : AwakenedRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;
    // TODO
}