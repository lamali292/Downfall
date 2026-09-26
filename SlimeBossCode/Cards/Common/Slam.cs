using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using SlimeBoss.SlimeBossCode.Core;

namespace SlimeBoss.SlimeBossCode.Cards.Common;

[Pool(typeof(SlimeBossCardPool))]
public class Slam : SlimeBossCardModel
{
    public Slam() : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithDamage(9, 1);
        WithPower<DrawCardsNextTurnPower>(1, 1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.ApplySelf<DrawCardsNextTurnPower>(ctx, this);
    }
}
