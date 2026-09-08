using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

public class MechaKnightCardPower : CollectorPowerModel
{
    public MechaKnightCardPower()
    {
        WithTip(StaticHoverTip.Channeling);
        WithTip(CardKeyword.Exhaust);
    }
    
    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature != Owner) return;
        var player = card.Owner;
        Flash();
        for (var i = 0; i < Amount; ++i)
        {
            var orb = OrbModel.GetRandomOrb(player.RunState.Rng.CombatOrbGeneration).ToMutable();
            await OrbCmd.Channel(ctx,orb, player);
        }
          
    }
}