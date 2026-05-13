using Awakened.AwakenedCode.Core;
using BaseLib.Abstracts;
using Downfall.DownfallCode.Interfaces;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Awakened.AwakenedCode.Powers;

public class PrimacyPower : AwakenedPowerModel, IHasSecondAmount
{
    private int StrengthGainsThisTurn => CombatManager.Instance.History.Entries
        .OfType<PowerReceivedEntry>()
        .Count(e => e.HappenedThisTurn(CombatState) &&
                    e.Actor == Owner &&
                    e is { Power: StrengthPower, Amount: > 0 });

    public string GetSecondAmount()
    {
        return $"{Math.Max(Amount - StrengthGainsThisTurn, 0)}";
    }

    public override async Task AfterPowerAmountChanged(PlayerChoiceContext ctx, PowerModel power, decimal amount,
        Creature? applier,
        CardModel? cardSource)
    {
        if (power is not StrengthPower || Owner.Player == null || power.Owner != Owner || amount <= 0 ||
            LocalContext.NetId == null)
            return;

        InvokeDisplayAmountChanged();

        if (StrengthGainsThisTurn > Amount) return;
        Flash();
        await CardPileCmd.Draw(ctx, Owner.Player);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}