using Awakened.AwakenedCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Awakened.AwakenedCode.Powers;

public class GreatHexPower() : AwakenedPowerModel(PowerType.Debuff)
{
    public override async Task AfterTurnEnd(PlayerChoiceContext ctx, CombatSide side)
    {
        if (Owner.Side != side) return;
        await PowerCmd.Apply<ManaburnPower>(ctx, Owner, Amount, Applier, null);
    }
}