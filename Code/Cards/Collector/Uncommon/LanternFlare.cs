using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Commands;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Uncommon;

[Pool(typeof(CollectorCardPool))]
public class LanternFlare : CollectorCardModel
{
    public LanternFlare() : base(2, CardType.Skill, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
        WithPyre();
        WithPower<CollectorDoomPower>(12, 3);
        WithPower<ScorchedPower>(3, 1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        await MyCommonActions.Apply<CollectorDoomPower>(this, cardPlay);
        await MyCommonActions.Apply<ScorchedPower>(this, cardPlay);
    }
}