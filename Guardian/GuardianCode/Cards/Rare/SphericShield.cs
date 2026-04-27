using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class SphericShield : GuardianCardModel
{
    public SphericShield() : base(3, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(10, 4);
        WithKeyword(CardKeyword.Exhaust);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await GuardianCmd.EnterDefensiveMode(ctx, Owner);
    }
}