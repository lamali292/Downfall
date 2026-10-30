using Hermit.HermitCode.Core;
using Hermit.HermitCode.CustomEnums;
using Hermit.HermitCode.Events;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Hermit.HermitCode.Relics;

/// <summary>
///     Spyglass — the 4th card you play each turn is Dead On, regardless of position.
/// </summary>
public sealed class Spyglass : HermitRelicModel, IShouldTriggerDeadOn
{
    public Spyglass() : base(RelicRarity.Shop)
    {
        WithTip(HermitKeywords.DeadOn);
        WithCards(3);
    }

    public override bool ShowCounter =>
        CombatManager.Instance.IsInProgress && CardsPlayedThisTurn <= DynamicVars.Cards.IntValue;

    // How many cards until the Dead On card. Shows the count-up toward 4.
    public override int DisplayAmount => CardsPlayedThisTurn;

    private int CardsPlayedThisTurn
    {
        get;
        set
        {
            AssertMutable();
            field = value;
            UpdateDisplay();
        }
    }

    public bool ShouldTriggerDeadOn(CardModel card)
    {
        if (card.Owner != Owner) return false;
        return CardsPlayedThisTurn + 1 == DynamicVars.Cards.IntValue;
    }

    private void UpdateDisplay()
    {
        // Light up when the NEXT card played will be the Dead On one.
        var next = CardsPlayedThisTurn + 1;
        Status = next == DynamicVars.Cards.IntValue ? RelicStatus.Active : RelicStatus.Normal;
        InvokeDisplayAmountChanged();
    }

    public override Task BeforeCombatStart()
    {
        CardsPlayedThisTurn = 0;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(
        CombatSide side,
        IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (!participants.Contains(Owner.Creature)) return Task.CompletedTask;
        CardsPlayedThisTurn = 0;
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }

    public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (cardPlay.Card.Owner == Owner && CombatManager.Instance.IsInProgress)
            CardsPlayedThisTurn++;
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        Status = RelicStatus.Normal;
        return Task.CompletedTask;
    }
}