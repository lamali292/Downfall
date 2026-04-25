using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Hexaghost.Multiplayer;

[Pool(typeof(HexaghostCardPool))]
public class EerieExpedition : HexaghostCardModel
{
    public EerieExpedition() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }


}