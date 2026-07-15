using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Guardian.GuardianCode.Relics;
//todo brace keyword
[Pool(typeof(GuardianRelicPool))]
public class BaalorsLordlyPlate() : GuardianRelicModel(RelicRarity.Common), IModifyBraceAmount
{
    public decimal ModifyBraceAmount(Player player, decimal amount)
    {
        return player == Owner ? amount + 1 : amount;
    }
}