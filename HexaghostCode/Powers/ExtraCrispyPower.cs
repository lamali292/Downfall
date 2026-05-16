using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Powers;

public class ExtraCrispyPower : HexaghostPowerModel, IAfterSoulburnDetonate
{
    public async Task AfterSoulburnDetonate(PlayerChoiceContext ctx, Creature creature)
    {
        if (Owner.CombatState == null || !Owner.CombatState.Enemies.Contains(creature)) return;
        await CreatureCmd.Damage(ctx, creature, Amount, ValueProp.Move | ValueProp.Unpowered | ValueProp.Unblockable,
            Owner);
        await PowerCmd.Apply<SoulBurnPower>(ctx, creature, Amount, Owner, null);
    }
}