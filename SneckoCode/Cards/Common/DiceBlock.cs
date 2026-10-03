using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class DiceBlock : SneckoCardModel
{
    public DiceBlock() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithOverflow();
        WithBlock(5, 2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var overflowing = OverflowCmd.OverflowActive(this);
        await CommonActions.CardBlock(this, cardPlay);
        await OverflowCmd.Overflow(overflowing, cardPlay, () => CommonActions.CardBlock(this, cardPlay));
    }
}