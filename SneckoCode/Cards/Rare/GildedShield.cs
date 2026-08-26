using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Cards.Rare;

[Pool(typeof(SneckoCardPool))]
public class GildedShield : SneckoCardModel
{
    public GildedShield() : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(8, 3);
        WithTip(SneckoKeywords.Muddle);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CardPileCmd.Add(this, PileType.Hand);
        await SneckoCmd.Muddle(ctx, this);
    }
}