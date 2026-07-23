using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Mimicry : SneckoCardModel
{
    public Mimicry() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        this.WithPower<MimicryPower>(2, 1, false);
        this.WithTip<StrengthPower>();
        WithTip(SneckoTip.Offclass);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MimicryPower>(ctx, this);
    }
}