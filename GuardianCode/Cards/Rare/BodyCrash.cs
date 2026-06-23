using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Cards.Rare;

[Pool(typeof(GuardianCardPool))]
public class BodyCrash : GuardianCardModel
{
    public BodyCrash() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithBlock(5, 3);
        WithCalculatedDamage(0, Calc);
        WithCalculatedVar("VisualBlock", 0, Calc2);
    }

    private static decimal Calc2(CardModel card, Creature? arg2)
    {
        return card.Owner.Creature.Block + card.DynamicVars.Block.PreviewValue;
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return card.Owner.Creature.Block;
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}