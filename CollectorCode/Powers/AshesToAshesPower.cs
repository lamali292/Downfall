using Collector.CollectorCode.Cards.Token;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class AshesToAshesPower : CollectorPowerModel
{
    public AshesToAshesPower()
    {
        WithTip<Ember>();
    }
    
    public override async Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Owner || card is not Ember)
            return;
        Flash();
        for (var i = 0; i < Amount; i++) CardCmd.Upgrade(card);
    }
    
}