using BaseLib.Utils;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class MudShield : SneckoCardModel
{
    public MudShield() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<MudShieldPower>(2, 1);
        WithTip(StaticHoverTip.Block);
        WithTip(SneckoKeywords.Muddle);
    }

    protected override Artist Artist => Artist.Get<Thelethargicweirdo>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MudShieldPower>(ctx, this);
    }
}