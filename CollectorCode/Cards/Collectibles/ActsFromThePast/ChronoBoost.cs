using BaseLib.Utils;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class ChronoBoost : ActsFromThePastCard
{
    public ChronoBoost() : base(1, CardType.Power, CardRarity.Rare, TargetType.Self, "TIME_EATER_BOSS")
    {
        WithPower<ChronoBoostPower>(2, 1, false);
        WithTip<StrengthPower>();
        WithVar("CardPlays", 12);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<ChronoBoostPower>(ctx, this);
    }
}