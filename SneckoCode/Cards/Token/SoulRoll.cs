using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;

namespace Snecko.SneckoCode.Cards.Token;

[Pool(typeof(TokenCardPool))]
public class SoulRoll : SneckoCardModel
{
    public SoulRoll() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self)
    {
        WithKeywords(CardKeyword.Retain, CardKeyword.Exhaust);
        WithBlock(3, 3);
        this.WithMuddle(1);
    }
    
    public override CardPoolModel VisualCardPool => ModelDb.CardPool<SneckoCardPool>();

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await SneckoCmd.MuddleHandCards(ctx, this);
    }
}