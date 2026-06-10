using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class TimeCapacitor : GuardianCardModel
{
    public TimeCapacitor() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithVar("StasisSlots", 1);
        WithTip(GuardianTip.Stasis);
    }

    protected override Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        GuardianCmd.AddMaxStasisSlots(Owner, DynamicVars["StasisSlots"].IntValue);
        return Task.CompletedTask;
    }
}