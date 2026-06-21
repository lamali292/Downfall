using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;

namespace Snecko.SneckoCode.Cards.Multiplayer;

[Pool(typeof(SneckoCardPool))]
public class SpreadTheChaos : SneckoCardModel
{
    public SpreadTheChaos() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
        this.WithMuddle(1, 1);
    }
    

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    
    
    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var cards = cardPlay.Target?.Player?
            .GetHand().OrderByDescending(e => e.EnergyCost.GetResolved())
            .Take(DynamicVars["Muddle"].IntValue);
        if (cards == null) return;
        await SneckoCmd.Muddle(ctx, cards, this);
    }
}