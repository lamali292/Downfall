using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class Condemn : CollectorCardModel
{
    public Condemn() : base(1, CardType.Skill, CardRarity.Common, TargetType.AnyEnemy)
    {
        WithPower<VulnerablePower>(1, 1);
        WithPower<CollectorDoomPower>(5, 1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.Apply<VulnerablePower>(ctx, this, cardPlay);
        await CommonActions.Apply<CollectorDoomPower>(ctx, this, cardPlay);
    }
}