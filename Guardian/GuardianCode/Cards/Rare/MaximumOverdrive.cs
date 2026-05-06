using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class MaximumOverdrive : GuardianCardModel
{
    public MaximumOverdrive() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithCostUpgradeBy(-1);
        WithPower<MaximumOverdrivePower>(1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MaximumOverdrivePower>(ctx, this);
    }
}