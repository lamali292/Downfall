using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Ancient;

[Pool(typeof(HexaghostCardPool))]
public class InfernalForm : HexaghostCardModel
{
    public InfernalForm() : base(3, CardType.Power, CardRarity.Ancient, TargetType.None)
    {
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }


}