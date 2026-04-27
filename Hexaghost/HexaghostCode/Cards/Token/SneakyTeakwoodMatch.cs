using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Hexaghost.HexaghostCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class SneakyTeakwoodMatch : HexaghostCardModel
{
    public SneakyTeakwoodMatch() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
    }


}