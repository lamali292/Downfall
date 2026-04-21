using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Basic;

[Pool(typeof(GuardianCardPool))]
public class DefendGuardian : GuardianCardModel
{
    public DefendGuardian() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithBlock(5, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}