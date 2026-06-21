using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class Restock : SneckoCardModel
{
    public Restock() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithKeyword(CardKeyword.Exhaust);
        WithCards(6);
        WithTip(SneckoKeywords.Muddle);
        WithCostUpgradeBy(-1);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = Owner.GetHand();
        await CardCmd.DiscardAndDraw(ctx, cards, DynamicVars.Cards.IntValue);
        var newCards = Owner.GetHand();
        await SneckoCmd.Muddle(ctx, newCards, this);
    }
}