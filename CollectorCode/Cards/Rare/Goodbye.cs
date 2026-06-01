using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.Powers;
using Downfall.DownfallCode.Artists;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Collector.CollectorCode.Cards.Rare;

[Pool(typeof(CollectorCardPool))]
public class Goodbye : CollectorCardModel
{
    public Goodbye() : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyEnemy)
    {
        WithCostUpgradeBy(-1);
    }

    protected override Artist Artist => Artist.Get<Opal>();


    protected override async Task OnPlay(PlayerChoiceContext ctx, CardPlay cardPlay)
    {
        if (cardPlay.Target is not { IsAlive: true }) return;
        var powerAmount = cardPlay.Target.GetPowerAmount<CollectorDoomPower>();
        if (powerAmount <= 0)
            return;
        await PowerCmd.Apply<CollectorDoomPower>(ctx, cardPlay.Target, powerAmount, Owner.Creature, this);
    }
}