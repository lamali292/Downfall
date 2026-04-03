using BaseLib.Utils;
using Downfall.Code.Cards.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Hexaghost.Token;

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


    protected override void OnUpgrade()
    {
    }
}