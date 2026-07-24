using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Mulligan : SneckoCardModel
{
    public Mulligan() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.AllEnemies)
    {
        this.WithPower<MulliganPower>(1, 1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MulliganPower>(ctx, this);
    }
}
