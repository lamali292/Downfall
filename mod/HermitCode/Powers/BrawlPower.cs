using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Powers;

public sealed class BrawlPower : HermitPowerModel
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        Flash();
        await PowerCmd.Apply<BruisePower>(choiceContext, CombatState.HittableEnemies, Amount, Owner, null);
    }
}