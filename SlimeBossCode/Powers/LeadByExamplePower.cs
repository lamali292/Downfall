using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Cards.Uncommon;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Powers;

public class LeadByExamplePower : SlimeBossPowerModel, IAfterCommand
{
    public async Task AfterCommand(PlayerChoiceContext ctx, Player player, SlimeModel slime, CardModel? source)
    {
        if (player.Creature != Owner || slime is not BruiserSlime) return;
        var slimes = player.Slimes.Where(s => s.Monster != slime);
        await PowerCmd.Apply<LeadByExamplePotencyPower>(ctx, slimes, Amount, Owner, source);
    }
}

public class LeadByExamplePotencyPower : CustomTemporaryPowerModelWrapper<LeadByExample, PotencyPower>;
