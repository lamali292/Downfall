using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Mimicry : SneckoCardModel
{
    public Mimicry() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithPower<MimicryPower>(2, 1, false);
        WithTip<StrengthPower>();
        WithTip(DownfallTip.Offclass);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<MimicryPower>(ctx, this);
    }
}