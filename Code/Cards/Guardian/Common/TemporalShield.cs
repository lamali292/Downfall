using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.DynamicVars;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Common;

[Pool(typeof(GuardianCardPool))]
public class TemporalShield : GuardianCardModel
{
    public TemporalShield() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(8, 3);
        WithAccelerate(1);
    }

    // TODO: Implement
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await GuardianCmd.Accelerate(this);
    }


    protected override void OnUpgrade()
    {
    }
}