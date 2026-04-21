using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Emergency : GuardianCardModel
{
    public Emergency() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithAccelerate(1, 1);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GuardianCmd.Accelerate(this, AccelerateType.All);
    }
}