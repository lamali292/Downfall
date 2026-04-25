using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Hexaghost.Token;

[Pool(typeof(TokenCardPool))]
public class ShadowGuise : HexaghostCardModel
{
    public ShadowGuise() : base(2, CardType.Power, CardRarity.Token, TargetType.None)
    {
        WithBlock(4, 2);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}