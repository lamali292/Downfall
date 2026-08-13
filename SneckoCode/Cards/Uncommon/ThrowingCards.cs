using BaseLib.Utils;
using Downfall.DownfallCode.CustomEnums;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class ThrowingCards : SneckoCardModel
{
    public ThrowingCards() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithTip(DownfallTip.Offclass);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (IsUpgraded) x++;
        await CommonActions.ApplySelf<ThrowingCardsPower>(ctx, this, x);
    }
}