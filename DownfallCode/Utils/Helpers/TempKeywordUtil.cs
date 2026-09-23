using BaseLib.Abstracts;
using Downfall.DownfallCode.Core;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;

namespace Downfall.DownfallCode.Utils;

public enum TempKeywordRemoveCondition
{
    /// Cleared when the card's owner's own turn ends.
    EndOfTurn,

    /// Cleared when the card's owner's own turn starts.
    StartOfTurn,

    /// Cleared when the enemy side's turn ends.
    EndOfEnemyTurn,

    /// Cleared when the enemy side's turn starts.
    StartOfEnemyTurn,

    /// Cleared the next time the card's owner spends any energy.
    EnergySpent,
}

/// <summary>
/// Grants a card a keyword "virtually", through the same TryModifyKeywordsInCombat mechanism
/// HexPower uses to grant Ethereal while a card is Hexed - instead of mutating the card's own
/// local keyword set via CardModel.AddKeyword/RemoveKeyword. This means callers never need to
/// worry about the card being a canonical/immutable instance, and there is nothing to undo if the
/// card gets cloned or transformed while a grant is active - the grant just stops applying once
/// its tracked entry is pruned.
/// </summary>
public class TempKeywordUtil() : CustomSingletonModel(HookType.Combat)
{
    private readonly record struct Entry(CardModel Card, CardKeyword Keyword, TempKeywordRemoveCondition Condition);

    private static readonly PlayerField<List<Entry>> Entries = new(_ => []);

    /// <summary>
    /// Grants <paramref name="card"/> <paramref name="keyword"/> until <paramref name="condition"/>
    /// clears it. Re-adding the same card/keyword pair replaces its condition (refreshes/extends
    /// the grant rather than stacking a second one).
    /// </summary>
    public static void Add(CardModel card, CardKeyword keyword, TempKeywordRemoveCondition condition)
    {
        var player = card.Owner;
        var list = Entries.Get(player);
        list?.RemoveAll(e => e.Card == card && e.Keyword == keyword);
        list?.Add(new Entry(card, keyword, condition));
    }

    /// Same as <see cref="Add(CardModel,CardKeyword,TempKeywordRemoveCondition)"/>, applied to every card in <paramref name="cards"/>.
    public static void Add(IEnumerable<CardModel> cards, CardKeyword keyword, TempKeywordRemoveCondition condition)
    {
        foreach (var card in cards) Add(card, keyword, condition);
    }

    /// Removes a specific temporary keyword grant early, regardless of its remove condition.
    public static void Remove(CardModel card, CardKeyword keyword)
    {
        Entries[card.Owner]?.RemoveAll(e => e.Card == card && e.Keyword == keyword);
    }

    /// Removes every temporary keyword grant on a card, e.g. when the card leaves combat for good.
    public static void ClearAll(CardModel card)
    {
        Entries[card.Owner]?.RemoveAll(e => e.Card == card);
    }

    /// Whether a temporary keyword grant is currently active on this card.
    public static bool Has(CardModel card, CardKeyword keyword)
    {
        return Entries[card.Owner]?.Any(e => e.Card == card && e.Keyword == keyword) ?? false;
    }

    public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
    {
        var list = Entries[card.Owner];
        return list is { Count: > 0 } && list
            .Where(entry => entry.Card == card)
            .Aggregate(false, (current, entry) => current | keywords.Add(entry.Keyword));
    }

    private static void RemoveForPlayer(Player player, TempKeywordRemoveCondition condition)
    {
        Entries[player]?.RemoveAll(e => e.Condition == condition);
    }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        RemoveForPlayer(player, TempKeywordRemoveCondition.StartOfTurn);
        return Task.CompletedTask;
    }

    public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Player) return Task.CompletedTask;
        foreach (var creature in participants)
            if (creature.Player is { } player)
                RemoveForPlayer(player, TempKeywordRemoveCondition.EndOfTurn);
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants,
        ICombatState combatState)
    {
        if (side != CombatSide.Enemy) return Task.CompletedTask;
        foreach (var player in combatState.Players)
            RemoveForPlayer(player, TempKeywordRemoveCondition.StartOfEnemyTurn);
        return Task.CompletedTask;
    }

    public override Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side,
        IEnumerable<Creature> participants)
    {
        if (side != CombatSide.Enemy) return Task.CompletedTask;
        // Unlike AfterSideTurnStart, this hook isn't handed an ICombatState - pull it off any
        // participant creature (all enemy-side creatures share one), falling back to the
        // current combat's state for the edge case where every enemy is already dead.
        var combatState = participants.FirstOrDefault()?.CombatState
                           ?? CombatManager.Instance.DebugOnlyGetState();
        if (combatState == null) return Task.CompletedTask;
        foreach (var player in combatState.Players)
            RemoveForPlayer(player, TempKeywordRemoveCondition.EndOfEnemyTurn);
        return Task.CompletedTask;
    }

    public override Task AfterEnergySpent(CardModel card, int amount)
    {
        var player = card.Owner;
        RemoveForPlayer(player, TempKeywordRemoveCondition.EnergySpent);
        return Task.CompletedTask;
    }

    public override Task AfterCombatEnd(CombatRoom room)
    {
        Entries.Clear();
        return Task.CompletedTask;
    }
}
