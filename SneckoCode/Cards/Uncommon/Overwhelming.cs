using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class Overwhelming : SneckoCardModel
{
    public Overwhelming() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithKeyword(CardKeyword.Ethereal, UpgradeType.Remove);
        this.WithPower<OverwhelmingPower>(1, false);
        WithTip(SneckoTip.Offclass);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<OverwhelmingPower>(ctx, this);
    }
}