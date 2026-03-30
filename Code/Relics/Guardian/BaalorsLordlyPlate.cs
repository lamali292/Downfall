using BaseLib.Utils;
using Downfall.Code.Abstract;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Downfall.Code.Relics.Guardian;

[Pool(typeof(GuardianRelicPool))]
public class BaalorsLordlyPlate : GuardianRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Common;
}