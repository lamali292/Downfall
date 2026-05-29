using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Basic;

[Pool(typeof(SneckoCardPool))]
public class TailWhip : SneckoCardModel, IHasOverflowEffect
{
    public TailWhip() : base(2, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        this.WithOverflow();
        WithDamage(10, 2);
        WithPower<WeakPower>(1, 1);
        WithPower<VulnerablePower>(1, 1);
    }

    public async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
    }

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}