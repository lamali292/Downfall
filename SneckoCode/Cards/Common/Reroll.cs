using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class Reroll : SneckoCardModel
{
    public Reroll() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(6 ,3);
        this.WithMuddle(1, 1);
    }
    

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await SneckoCmd.MuddleHandCards(ctx, this);
    }
}