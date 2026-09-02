using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.CustomEnums;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Reroll : SneckoCardModel
{
    public Reroll() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6, 3);
        WithKeyword(SneckoKeywords.Muddle);
    }


    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var maxCost = Owner.Hand.Max(e => e.EnergyCost.GetResolved());
        var highestCostCards = Owner.Hand.Where(e => e.EnergyCost.GetResolved() == maxCost).ToList();
        var card = RunState!.Rng.CombatCardSelection.NextItem(highestCostCards);
        if (card == null) return;
        await SneckoCmd.Muddle(ctx, card, this);
    }
}