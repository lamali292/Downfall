using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.DynamicVars;
using Guardian.GuardianCode.Events;
using Guardian.GuardianCode.Extensions;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Guardian.GuardianCode.Relics;

[Pool(typeof(GuardianRelicPool))]
public class BaalorsLordlyPlate : GuardianRelicModel, IModifyBraceAmount
{
    public BaalorsLordlyPlate() : base(RelicRarity.Common)
    {
        WithTip(GuardianTip.Brace);
        WithVars(new BraceVar(1));
    }

    public decimal ModifyBraceAmount(Player player, decimal amount)
    {
        return player == Owner ? amount + DynamicVars.Brace().BaseValue : amount;
    }
}