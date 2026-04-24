using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Common;

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