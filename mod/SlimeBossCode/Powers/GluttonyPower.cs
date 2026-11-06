using Downfall.DownfallCode.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using SlimeBoss.SlimeBossCode.Cards.Token;
using SlimeBoss.SlimeBossCode.Core;
using SlimeBoss.SlimeBossCode.CustomEnums;
using SlimeBoss.SlimeBossCode.Events;
using SlimeBoss.SlimeBossCode.History;

namespace SlimeBoss.SlimeBossCode.Powers;

public class GluttonyPower : SlimeBossPowerModel, IAfterConsumeEffect
{
    public GluttonyPower()
    {
        WithTip<Lick>();
        WithTip(SlimeBossTip.Consume);
    }

    private int ConsumeThisTurn => CombatManager.Instance.History.Entries.OfType<ConsumeEntry>()
        .Count(e => e.Actor == Owner && e.HappenedThisTurn(CombatState));

    public async Task AfterConsumeEffect(PlayerChoiceContext ctx, Creature creature, Creature attacker, decimal amount)
    {
        if (attacker != Owner || Owner.Player == null || ConsumeThisTurn > Amount) return;
        await DownfallCardCmd.GiveCards<Lick>(Owner.Player, PileType.Hand, 1);
    }
}