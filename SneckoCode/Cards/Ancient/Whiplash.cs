using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Ancient;

[Pool(typeof(SneckoCardPool))]
public class Whiplash : SneckoCardModel
{
    public Whiplash() : base(2, CardType.Attack, CardRarity.Ancient, TargetType.AllEnemies)
    {
        WithDamage(14, 4);
        WithPower<WeakPower>(2, 1);
        WithPower<VulnerablePower>(2, 1);
    }
 
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
    }
}