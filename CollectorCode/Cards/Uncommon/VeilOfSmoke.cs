using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class VeilOfSmoke : CollectorCardModel
{
    public VeilOfSmoke() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<VeilOfSmokePower>(5, 2);
        WithTip(StaticHoverTip.Block);
        WithKeyword(CardKeyword.Exhaust);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var turns = 1 + Owner.Hand.Count(e => e.Type == CardType.Status);
        (await CommonActions.ApplySelf<VeilOfSmokePower>(ctx, this, turns))?
            .SetBlock(DynamicVars.Power<VeilOfSmokePower>().IntValue);
    }
}