using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class RepulsorGuardian : GuardianCardModel
{
    public RepulsorGuardian() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(CardKeyword.Exhaust);
        WithPower<ExhaustStatusesPower>(1, false);
        WithCostUpgradeBy(-1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ExhaustStatusesPower>(ctx, this);
    }
}