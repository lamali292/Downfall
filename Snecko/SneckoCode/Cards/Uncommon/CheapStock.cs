using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;
using Snecko.SneckoCode.Powers;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class CheapStock : SneckoCardModel
{
    public CheapStock() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.None)
    {
        WithPower<CheapStockPower>(1, 1);
        WithTip(SneckoKeywords.Muddle);
    }
    
    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<CheapStockPower>(ctx, this);
    }
}