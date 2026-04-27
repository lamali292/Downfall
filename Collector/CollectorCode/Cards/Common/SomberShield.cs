using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Common;

[Pool(typeof(CollectorCardPool))]
public class SomberShield : CollectorCardModel
{
    public SomberShield() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithPyre();
        WithBlock(6, 3);
        WithPower<CopyNextTurnPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        var a = await CommonActions.ApplySelf<CopyNextTurnPower>(ctx, this);
        if (a == null || PyredCard == null) return;
        a.Card = PyredCard.CreateClone();
    }
}