using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class WeightedDice : SneckoCardModel
{

    public WeightedDice() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
    {
        WithCostUpgradeBy(-1);
        WithTip(SneckoKeywords.Muddle);
        this.WithPower<WeightedDicePower>(1, false);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<WeightedDicePower>(ctx, this);
    }
    
}