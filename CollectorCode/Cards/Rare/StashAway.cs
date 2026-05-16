using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class StashAway : CollectorCardModel
{
    public StashAway() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
    {
        WithBlock(4, 2);
        WithTip(typeof(ReserveNextTurnPower));
        WithKeyword(CardKeyword.Exhaust);
    }


    protected override bool HasEnergyCostX => true;

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        var x = ResolveEnergyXValue();
        for (var i = 0; i < x; i++)
            await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<ReserveNextTurnPower>(ctx, this, x);
    }
}