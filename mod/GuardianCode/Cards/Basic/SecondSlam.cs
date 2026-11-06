using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Guardian.GuardianCode.Cards.Basic;

[Pool(typeof(GuardianCardPool))]
public class SecondSlam : GuardianCardModel, IGemSocketCard
{
    public SecondSlam() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Ethereal, CardKeyword.Exhaust);
        WithDamage(7);
    }

    public int GemSlots => IsUpgraded ? 2 : 1;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}