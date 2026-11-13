using Downfall.DownfallCode.Artists;
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
        WithPower<MulliganPower>(1, false);
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
    }

    protected override Artist Artist => Artist.Get<Fulgur>();
    protected override bool IsBetaArt => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MulliganPower>(ctx, this);
    }
}