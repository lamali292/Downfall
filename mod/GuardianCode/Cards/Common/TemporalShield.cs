using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class TemporalShield : GuardianCardModel
{
    public TemporalShield() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(7, 3);
        WithAccelerate(1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await GuardianCmd.Accelerate(ctx, this);
    }
}