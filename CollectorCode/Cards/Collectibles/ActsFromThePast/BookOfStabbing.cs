using BaseLib.Utils;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class BookOfStabbing : ActsFromThePastCard
{
    public BookOfStabbing() : base(3, CardType.Power, CardRarity.Uncommon, TargetType.Self, "BOOK_OF_STABBING_ELITE")
    {
        WithCostUpgradeBy(-1);
        WithPower<BookOfStabbingPower>(1, false);
        WithTip<MiasmaPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<BookOfStabbingPower>(ctx, this);
    }
}