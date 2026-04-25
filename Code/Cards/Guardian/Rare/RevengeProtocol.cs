using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Downfall.Code.Cards.Guardian.Rare;

[Pool(typeof(GuardianCardPool))]
public class RevengeProtocol : GuardianCardModel
{
    public RevengeProtocol() : base(2, CardType.Power, CardRarity.Rare, TargetType.None)
    {
        WithPower<BracingPower>(4, 2);
        WithPower<RevengeProtocolPower>(2, 1);
        WithTip(typeof(StrengthPower));
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<RevengeProtocolPower>(ctx, this);
        await CommonActions.ApplySelf<BracingPower>(ctx, this);
    }
}