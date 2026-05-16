using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Hexaghost.HexaghostCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class ShadowGuise : HexaghostCardModel
{
    public ShadowGuise() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithBlock(4, 2);
        WithKeywords(CardKeyword.Exhaust, CardKeyword.Retain);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}