using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class WideAngle : SneckoCardModel
{
    public WideAngle() : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        WithKeyword(CardKeyword.Retain);
        WithDamage(18, 6);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
    }
}