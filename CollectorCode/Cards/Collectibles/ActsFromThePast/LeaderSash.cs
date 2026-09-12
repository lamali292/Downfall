using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class LeaderSash : ActsFromThePastCard
{
    public LeaderSash() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self, "GREMLIN_LEADER_ELITE")
    {
        WithPower<LeaderSashPower>(2, 1, false);
        WithCards(2, 1);
        WithTip<StrengthPower>();
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.ApplySelf<LeaderSashPower>(ctx, this);
        await CommonActions.Draw(this, ctx);
    }
}

public class LeaderSashPower : CustomTemporaryPowerModelWrapper<LeaderSash, StrengthPower>;