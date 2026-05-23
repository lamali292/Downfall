using Downfall.DownfallCode.Powers;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Powers;

public class GhostflameBarrierPower : HexaghostPowerModel
{
    public override async Task AfterDamageReceived(PlayerChoiceContext choiceContext, Creature target, DamageResult _,
        ValueProp props, Creature? dealer, CardModel? __)
    {
        if (target == Owner && dealer != null && props.IsPoweredAttack())
            await PowerCmd.Apply<SoulBurnPower>(choiceContext, dealer, Amount, Owner, null);
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side == Owner.Side) return;
        await PowerCmd.Remove(this);
    }
}