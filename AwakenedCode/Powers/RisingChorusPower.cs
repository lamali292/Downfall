using Awakened.AwakenedCode.Core;
using Awakened.AwakenedCode.CustomEnums;
using Awakened.AwakenedCode.Events;
using Awakened.AwakenedCode.History;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;

namespace Awakened.AwakenedCode.Powers;

public class RisingChorusPower : AwakenedPowerModel, IModifyChantRepeatCount
{
    public override int DisplayAmount => Math.Max(Amount - ChantThisTurn, 0);

    private int ChantThisTurn => CombatManager.Instance.History.Entries.OfType<ChantEntry>()
        .Count(e => e.HappenedThisTurn(CombatState) && e.Actor == Owner);

    public int ModifyChantRepeatCount(CardModel card, CardPlay cardPlay, int count)
    {
        if (card.Owner.Creature != Owner || !card.Keywords.Contains(AwakenedKeyword.Chant)) return count;
        return ChantThisTurn <= Amount ? count + 1 : count;
    }

    public Task AfterModifyingChantRepeatCount(CardModel card, CardPlay cardPlay)
    {
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player.Creature != Owner) return Task.CompletedTask;
        InvokeDisplayAmountChanged();
        return Task.CompletedTask;
    }
}