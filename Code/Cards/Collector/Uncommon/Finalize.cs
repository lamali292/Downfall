using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class Finalize : CollectorCardModel
{
    public Finalize() : base(4, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithKeywords(CardKeyword.Exhaust);
        WithPower<CollectorDoomPower>(24, 4);
        WithPower<FinalizePower>(6, 2);
    }

    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<CollectorDoomPower>(this, cardPlay);
        await MyCommonActions.Apply<FinalizePower>(this, cardPlay);
    }
}