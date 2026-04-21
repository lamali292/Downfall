using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Guardian.Basic;

[Pool(typeof(GuardianCardPool))]
public class SecondSlam : GuardianCardModel
{
    public SecondSlam() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Ethereal, CardKeyword.Exhaust);
        WithDamage(7);
        WithUpgradedCardTip<TwinSlam>((c, g) =>
        {
            if (g is GuardianCardModel other)
                c.AddGems(other.Gems);
        });
    }

    public override int GemSlots => IsUpgraded ? 2 : 1;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}