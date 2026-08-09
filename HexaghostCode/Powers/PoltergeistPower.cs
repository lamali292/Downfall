using Downfall.DownfallCode.Compatibility;
using Hexaghost.HexaghostCode.Core;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Hexaghost.HexaghostCode.Powers;

public class PoltergeistPower : HexaghostPowerModel
{
    public override async Task AfterCardExhausted(PlayerChoiceContext ctx, CardModel card, bool causedByEthereal)
    {
        if (card.Owner.Creature != Owner) return;
        var creature = CombatState.RunState.Rng.CombatTargets.NextItem(CombatState.HittableEnemies);
        if (creature == null) return;
        await DownfallCreatureCmd.Damage(ctx, creature, Amount,
            DamageProps.nonCardHpLoss, Owner, null, null);
        Flash();
    }
}