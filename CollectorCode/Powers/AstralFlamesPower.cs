using Collector.CollectorCode.Extensions;
using Collector.CollectorCode.CustomEnums;
using Collector.CollectorCode.Core;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Collector.CollectorCode.Powers;

/// <summary>Equip: Bellows: every card you Exhaust makes Torchhead strike.</summary>
public class AstralFlamesPower : CollectorPowerModel
{
    public AstralFlamesPower()
    {
        WithTip(CollectorKeyword.Torchhead);
    }

    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        var player = Owner.Player;
        if (player == null || card.Owner != player) return;
        Flash();
        await TorchheadCmd.TorchheadAttack(player, Amount).ExecuteIfPresent(ctx);
    }
}
