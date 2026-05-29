using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Blunderbus : SneckoCardModel
{
    public Blunderbus() : base(1, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies)
    {
        WithDamage(8, 3);
    }

    private int ThreeCostInHand => Owner.GetHand().Count(e => e.EnergyCost.GetResolved() >= 3);

    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay, 1 + ThreeCostInHand).Execute(ctx);
    }
}