using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class EvasiveProtocol : GuardianCardModel
{
    public EvasiveProtocol() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithBrace(6, 3);
        WithPower<EvasiveProtocolPower>(1, 1, false);
        WithTip(GuardianTip.Polish);
        WithTip(GuardianTip.DefensiveMode);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Brace(ctx, this);
        await CommonActions.ApplySelf<EvasiveProtocolPower>(ctx, this);
    }
}