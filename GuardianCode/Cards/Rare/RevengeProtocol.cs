using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class RevengeProtocol : GuardianCardModel
{
    public RevengeProtocol() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<BracingPower>(4, 2, false);
        WithPower<RevengeProtocolPower>(2, 1, false);
        WithTip(typeof(StrengthPower));
        WithTip(GuardianTip.DefensiveMode);
        WithTip(GuardianTip.Brace);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RevengeProtocolPower>(ctx, this);
        await CommonActions.ApplySelf<BracingPower>(ctx, this);
    }
}