using BaseLib.Utils;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;

namespace Downfall.Code.Cards.Guardian.Token;

[Pool(typeof(TokenCardPool))]
public class CrystalWard : GuardianCardModel
{
    public CrystalWard() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithBlock(4, 2);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}