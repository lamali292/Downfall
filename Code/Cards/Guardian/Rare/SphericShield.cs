using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Rare;

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
        await GuardianCmd.EnterDefensiveMode(Owner);
    }
}