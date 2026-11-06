using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Snecko.SneckoCode.Core;

namespace Snecko.SneckoCode.Cards.Common;

[Pool(typeof(SneckoCardPool))]
public class DiceCrush : SneckoCardModel
{
    public DiceCrush() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithOverflow();
        WithDamage(18, 4);
        WithCards(2);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var overflowing = OverflowCmd.OverflowActive(this);
        await CommonActions.CardAttack(this, cardPlay).Execute(ctx);
        await OverflowCmd.Overflow(overflowing, cardPlay, () => CommonActions.Draw(this, ctx));
    }
}