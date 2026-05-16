using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class Arrogance : CollectorCardModel
{
    public Arrogance() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPower<DoomedAttackPower>(4, 1);
        WithBlock(7, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<DoomedAttackPower>(ctx, this);
    }
}