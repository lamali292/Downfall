using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using Gremlins.GremlinsCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Gremlins.GremlinsCode.Cards.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class Stupend : GremlinsCardModel
{
    public Stupend() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithCalculatedBlock(7, Calc, BlockProps.card, 2);
        WithDamage(7, 2);
    }

    private static decimal Calc(CardModel card, Creature? creature)
    {
        return card.Owner.Creature.GetPowerAmount<BangPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}