using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Players;

namespace Hexaghost.HexaghostCode.Powers;

public class PremonitionPower : HexaghostPowerModel
{
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        var exhausted = CombatManager.Instance.History.Entries
            .OfType<CardExhaustedEntry>()
            .Count(e => e.Actor == Owner && e.HappenedLastPlayerTurn(player));
        return exhausted >= 2 ? count + 2 : count;
    }

    public override Task AfterModifyingHandDraw()
    {
        Flash();
        return Task.CompletedTask;
    }
}