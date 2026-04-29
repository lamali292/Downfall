using BaseLib.Utils;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Cards.Ancient;

[Pool(typeof(HexaghostCardPool))]
public class InfernalForm : HexaghostCardModel
{
    public InfernalForm() : base(3, CardType.Power, CardRarity.Ancient, TargetType.None)
    {
        WithPower<InfernalFormPower>(2, 3);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<InfernalFormPower>(ctx, this);
    }
}