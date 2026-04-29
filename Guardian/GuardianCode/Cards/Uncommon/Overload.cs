using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Uncommon;

[Pool(typeof(GuardianCardPool))]
public class Overload : GuardianCardModel
{
    public Overload() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithCards(2, 1);
        WithPolish(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Draw(this, ctx);
        await GuardianCmd.Polish(ctx, this);
    }
}