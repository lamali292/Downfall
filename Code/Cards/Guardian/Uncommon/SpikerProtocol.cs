using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Guardian;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class SpikerProtocol : GuardianCardModel
{
    public SpikerProtocol() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<SpikerProtocolPower>(2, 1);
        WithBrace(6, 3);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<SpikerProtocolPower>(ctx, this);
        await GuardianCmd.Brace(ctx, this);
    }
}