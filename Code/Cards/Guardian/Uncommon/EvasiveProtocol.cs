using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class EvasiveProtocol : GuardianCardModel
{
    public EvasiveProtocol() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithBrace(6, 3);
        WithPower<EvasiveProtocolPower>(1, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Brace(this);
        await CommonActions.ApplySelf<EvasiveProtocolPower>(this);
    }
}