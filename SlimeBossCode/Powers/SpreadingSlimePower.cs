using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.Extensions;
using SlimeBoss.SlimeBossCode.Slimes;

namespace SlimeBoss.SlimeBossCode.Powers;

/// <summary>Whenever the owner creates a Status card, Bruiser Slime gains Potency (no effect while it doesn't exist).</summary>
public class SpreadingSlimePower : SlimeBossPowerModel
{
    public SpreadingSlimePower()
    {
        WithTip<PotencyPower>();
        WithSlimeTip<BruiserSlime>();
    }

    public override Task AfterCardGeneratedForCombat(CardModel card, Player? creator)
    {
        if (creator == null || creator.Creature != Owner || card.Type != CardType.Status) return Task.CompletedTask;
        var slime = card.Owner.GetSlime<BruiserSlime>();
        return slime == null ? Task.CompletedTask : PowerCmd.Apply<PotencyPower>(new BlockingPlayerChoiceContext(), slime, Amount, Owner, null);
    }
}
