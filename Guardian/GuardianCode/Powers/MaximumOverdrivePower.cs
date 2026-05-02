using Downfall.DownfallCode.Powers.Downfall;
using Guardian.GuardianCode.Core;
using Guardian.GuardianCode.Events;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Guardian.GuardianCode.Powers;

public class MaximumOverdrivePower : GuardianPowerModel, IAfterCardTick
{
    public async Task AfterCardTick(PlayerChoiceContext ctx, CardModel card, Player player)
    {
        if (player.Creature != Owner) return;
        await PowerCmd.Apply<TemporaryStrengthUpPower>(ctx, player.Creature, Amount, player.Creature, null);
    }
}