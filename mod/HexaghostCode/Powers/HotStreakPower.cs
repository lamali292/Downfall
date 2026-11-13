using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hexaghost.HexaghostCode.Powers;

public class HotStreakPower : HexaghostPowerModel
{
    public override async Task AfterSideTurnEnd(PlayerChoiceContext ctx, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != Owner.Side) return;
        foreach (var enemy in CombatState.HittableEnemies)
        {
            var soulburn = enemy.GetPower<SoulBurnPower>();
            if (soulburn == null) continue;
            await PowerCmd.ModifyAmount(ctx, soulburn, Amount, Owner, null);
        }
    }
}