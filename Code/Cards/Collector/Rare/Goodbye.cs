using BaseLib.Utils;
using Downfall.Code.Abstract;
using Downfall.Code.Abstract.CardModels;
using Downfall.Code.Powers.Collector;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Downfall.Code.Cards.Collector.Rare;

[Pool(typeof(CollectorCardPool))]
public class Goodbye : CollectorCardModel
{
    public Goodbye() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithCostUpgradeBy(-1);
    }


    protected override async Task PlayEffect(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { IsAlive: true }) return;
        var powerAmount = cardPlay.Target.GetPowerAmount<CollectorDoomPower>();
        if (powerAmount <= 0)
            return;
        await PowerCmd.Apply<CollectorDoomPower>(ctx, cardPlay.Target, powerAmount, Owner.Creature, this);
    }
}