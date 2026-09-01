using BaseLib.Extensions;
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
        WithPower<ThrowingCardsPower>(8, false);
    }

    protected override bool HasEnergyCostX => true;

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        if (IsUpgraded) x++;
        var power = await CommonActions.ApplySelf<ThrowingCardsPower>(ctx, this, x);
        power?.SetDamage(DynamicVars.Power<ThrowingCardsPower>().BaseValue);
        power?.CardPlay = cardPlay;
    }
}