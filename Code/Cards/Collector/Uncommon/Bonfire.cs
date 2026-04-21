using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Bonfire : CollectorCardModel
{
    public Bonfire() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
    {
        WithPyre();
        WithBlock(12, 4);
        WithPower<ReserveNextTurnPower>(1);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);
        await CommonActions.ApplySelf<ReserveNextTurnPower>(this);
    }
}