using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Common;

[Pool(typeof(CollectorCardPool))]
public class Misdirect : CollectorCardModel
{
    public Misdirect() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self)
    {
        WithBlock(2, 2);
        WithPower<ReserveNextTurnPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<ReserveNextTurnPower>(this);
    }
}