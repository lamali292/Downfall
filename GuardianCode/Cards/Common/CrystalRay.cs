using BaseLib.Utils;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.CustomEnums;
using Guardian.GuardianCode.Interfaces;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Guardian.GuardianCode.Cards.Common;

[Pool(typeof(GuardianCardPool))]
public class CrystalRay : GuardianCardModel
{
    public CrystalRay() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithTip(GuardianKeyword.Gem);
        WithCalculatedDamage(12, 2, Calc, ValueProp.Move, 4, 1);
    }

    private static decimal Calc(CardModel card, Creature? creature)
    {
        return PileType.Deck.GetPile(card.Owner).Cards.OfType<IGemSocketCard>().Sum(g => g.GemCount);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}
