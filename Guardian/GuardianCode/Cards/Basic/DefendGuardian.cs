using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Basic;

[Pool(typeof(GuardianCardPool))]
public class DefendGuardian : GuardianCardModel
{
    public DefendGuardian() : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
        WithTags(CardTag.Defend);
        WithBlock(5, 3);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}