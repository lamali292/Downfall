using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     Horror: Bruise does not wear off for X turns.
/// </summary>
public sealed class HorrorPower() : HermitPowerModel(PowerType.Debuff)
{
    public override async Task AfterTurnEndLate(PlayerChoiceContext ctx, CombatSide side)
    {
        if (side == Owner.Side)
            await PowerCmd.Apply<HorrorPower>(ctx, Owner, -1, Owner, null);
    }
}