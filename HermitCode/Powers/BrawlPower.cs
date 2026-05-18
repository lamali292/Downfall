using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Powers;

/// <summary>
///     At the start of your turn, apply X Bruise to ALL enemies.
/// </summary>
public sealed class BrawlPower : HermitPowerModel
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        Flash();

        foreach (var enemy in CombatState.HittableEnemies)
            await PowerCmd.Apply<BruisePower>(choiceContext, enemy, Amount, Owner, null);
    }
}