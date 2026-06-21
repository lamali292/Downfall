using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class SoulExchange : SneckoCardModel
{
    public SoulExchange() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithCostUpgradeBy(-1);
        WithTip(SneckoKeywords.Muddle);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await SneckoCmd.Muddle(ctx, Owner.GetHand(), this);
    }
}