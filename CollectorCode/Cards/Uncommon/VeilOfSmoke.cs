using BaseLib.Extensions;
using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Cards.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class VeilOfSmoke : CollectorCardModel
{
    public VeilOfSmoke() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<VeilOfSmokePower>(6, 2, false);
        WithTip(StaticHoverTip.Block);
        WithKeyword(CardKeyword.Exhaust);
        WithCalculatedVar("Turns",1, Calc);
    }
    
    private static decimal Calc(CardModel card, Creature? creature)
    {
        return card.Owner.Hand.Count(c => c.Type is CardType.Status);
    }

    protected override async Task OnPlayInternal(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var turns = ((CalculatedVar)DynamicVars["Turns"]).Calculate(cardPlay.Target);
        (await CommonActions.ApplySelf<VeilOfSmokePower>(ctx, this, turns))?
            .SetBlock(DynamicVars.Power<VeilOfSmokePower>().IntValue);
    }
}