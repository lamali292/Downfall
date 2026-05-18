using Hermit.HermitCode.Core;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;

namespace Hermit.HermitCode.Powers;

public sealed class TakeAimPower : HermitPowerModel
{
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner.Player) return;
        Flash();
        await PowerCmd.Apply<ConcentrationPower>(choiceContext, Owner, Amount, Owner, null);
    }
}