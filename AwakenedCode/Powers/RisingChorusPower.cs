using System.Globalization;
using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.History;
using Awakened.AwakenedCode.Interfaces;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Powers;

public class RisingChorusPower : AwakenedPowerModel, IOnChant
{
    public override int DisplayAmount => Math.Max(Amount - ChantThisTurn, 0);

    private int ChantThisTurn => CombatManager.Instance.History.Entries.OfType<ChantEntry>()
        .Count(e => e.HappenedThisTurn(CombatState) && e.FirstChantInSeries);
    
    public async Task OnCardChanted(CardModel card, PlayerChoiceContext ctx, CardPlay cardPlay, bool firstTime)
    {
        if (card.Owner.Creature != Owner || card is not IChantable) return;
        if (ChantThisTurn <= Amount && firstTime)
        {
            await AwakenedCmd.Chant(ctx, card, cardPlay, false);
        }
        InvokeDisplayAmountChanged();
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}