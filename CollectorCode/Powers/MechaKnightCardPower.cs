using BaseLib.Utils;
using Collector.CollectorCode.Core;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
namespace Collector.CollectorCode.Powers;

[Pool(typeof(TokenCardPool))]
public class MechaKnightCardPower : CollectorPowerModel, IAfterCardPyred
{
    public MechaKnightCardPower()
    {
        WithTip(StaticHoverTip.Channeling);
        WithTip(CollectorTip.Pyred);
    }

    public async Task AfterCardPyred(PlayerChoiceContext ctx, CardModel card, CardModel pyred)
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