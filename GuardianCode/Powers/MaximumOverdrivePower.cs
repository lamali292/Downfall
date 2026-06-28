using BaseLib.Abstracts;
using Downfall.DownfallCode.Powers;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Guardian.GuardianCode.Powers;

public class MaximumOverdrivePower : GuardianPowerModel, IAfterCardTick
{
    public async Task AfterCardTick(PlayerChoiceContext ctx, CardModel card, Player player)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<MaximumOverdrivePowerPower>(ctx, player.Creature, Amount, player.Creature, null);
    }
}

public class MaximumOverdrivePowerPower : CustomTemporaryPowerModelWrapper<MaximumOverdrivePower, StrengthPower>;