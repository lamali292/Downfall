using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;
using Snecko.SneckoCode.Extensions;
using Snecko.SneckoCode.Interfaces;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class DiceBlock : SneckoCardModel, IHasOverflowEffect
{
    public DiceBlock() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        this.WithOverflow();
        WithBlock(5, 2);
    }

    public async Task OverflowEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
    }
}