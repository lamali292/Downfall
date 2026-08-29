using Downfall.DownfallCode.Compatibility;
using Hexaghost.HexaghostCode.Core;
using Hexaghost.HexaghostCode.CustomEnums;
using Hexaghost.HexaghostCode.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Powers;

public class PoltergeistPower : HexaghostPowerModel, IWheelMoved
{
    public PoltergeistPower()
    {
        WithTip(HexaghostKeyword.Advance);
        WithTip(HexaghostKeyword.Retract);
    }

    public Task AfterWheelAdvance(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        return DamageAction(ctx, player);
    }

    public Task AfterWheelRetract(PlayerChoiceContext ctx, Player player, AbstractModel? source,
        GhostflameModel ghostflame,
        int ghostflameIndex, bool silent)
    {
        return DamageAction(ctx, player);
    }

    private async Task DamageAction(PlayerChoiceContext ctx, Player player)
    {
        if (player.Creature != Owner) return;
        var creature = CombatState.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (creature == null) return;
        await CompatibilityCreatureCmd.Damage(ctx, creature, Amount,
            DamageProps.nonCardHpLoss, Owner, null, null);
        Flash();
    }
}