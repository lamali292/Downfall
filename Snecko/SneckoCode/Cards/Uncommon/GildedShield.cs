using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Cards.Uncommon;

[Pool(typeof(SneckoCardPool))]
public class GildedShield : SneckoCardModel
{
    public GildedShield() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithBlock(8, 3);
        WithTip(SneckoKeywords.Muddle);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await SneckoCmd.Muddle(ctx, this);
        await CardPileCmd.Add(this, PileType.Hand);
    }
}