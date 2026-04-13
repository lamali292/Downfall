using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Cards.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Common;

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
        await CommonActions.ApplySelf<DoomedAttackPower>(this);
    }
}