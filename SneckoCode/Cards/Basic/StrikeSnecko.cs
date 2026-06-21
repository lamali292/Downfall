using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Basic;

[Pool(typeof(SneckoCardPool))]
public class StrikeSnecko : SneckoCardModel
{
    public StrikeSnecko() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
        WithTags(CardTag.Strike);
        WithDamage(6, 3);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}