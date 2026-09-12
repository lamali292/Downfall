using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Collectibles.ActsFromThePast;

public class YouYourself : ActsFromThePastCard
{
    public YouYourself() : base(0, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy, "COLLECTOR_BOSS")
    {
        WithPower<WeakPower>(1);
        WithPower<VulnerablePower>(1);
        WithPower<MiasmaPower>(1);
        WithKindle(1);
        WithKeyword(CardKeyword.Exhaust, UpgradeType.Remove);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<WeakPower>(ctx, this, cardPlay);
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
        await CommonActions.Apply<MiasmaPower>(ctx, this, cardPlay);
        await CollectorCmd.Kindle(ctx, this);
    }
}