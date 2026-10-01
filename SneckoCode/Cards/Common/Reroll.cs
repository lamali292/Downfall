using Downfall.DownfallCode.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
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
        var candidates = Owner.Hand.Where(e => e.HasNumericCost).ToList();
        if (candidates.Count == 0) return;
        var maxCost = candidates.Max(e => e.EffectiveCost);
        var highestCostCards = candidates.Where(e => e.EffectiveCost == maxCost).ToList();
        var card = RunState!.Rng.CombatCardSelection.NextItem(highestCostCards);
        if (card == null) return;
        await SneckoCmd.Muddle(ctx, card, this);
    }
}