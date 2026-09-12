using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class LagavulinClaw : ActsFromThePastCard
{
    public LagavulinClaw() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.AllEnemies, "LAGAVULIN_ELITE")
    {
        WithPower<WeakPower>(1);
        WithPower<LagavulinClawPower>(2, 4, false);
        WithTip<StrengthPower>();
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await CommonActions.Apply<LagavulinClawPower>(ctx, this, cardPlay);
    }
}

public class LagavulinClawPower() : CustomTemporaryPowerModelWrapper<LagavulinClaw, StrengthPower>
{
    protected override bool InvertInternalPowerAmount => true;
}