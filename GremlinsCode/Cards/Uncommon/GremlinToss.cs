using BaseLib.Utils;
using Gremlins.GremlinsCode.Core;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Gremlins.GremlinsCode.Cards.Uncommon;

[Pool(typeof(GremlinsCardPool))]
public class GremlinToss : GremlinsCardModel
{
    public GremlinToss() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        this.WithTempHp(3);
        WithCostUpgradeBy(-1);
        WithCalculatedDamage(0, Calc);
    }

    private static decimal Calc(CardModel card, Creature? arg2)
    {
        return GremlinsCmd.GetTempHpAmount(card.Owner.Creature) + card.Owner.Creature.Block;
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await GremlinsCmd.GainTempHp(ctx, this);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}