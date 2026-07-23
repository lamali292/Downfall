using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class UnendingSupply : SneckoCardModel
{
    public UnendingSupply() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(DownfallKeyword.Echo);
        WithTip(CardKeyword.Ethereal);
        WithTip(CardKeyword.Exhaust);
        this.WithPower<UnendingSupplyPower>(1, false);
        WithKeyword(CardKeyword.Innate, UpgradeType.Add);
        WithTip(SneckoTip.Offclass);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<UnendingSupplyPower>(ctx, this);
    }
}