using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class FuturePlans : GuardianCardModel
{
    public FuturePlans() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<FuturePlansPower>(1, false);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip(GuardianTip.Stasis);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<FuturePlansPower>(ctx, this);
    }
}